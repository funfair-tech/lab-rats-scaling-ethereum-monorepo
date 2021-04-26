pragma solidity >=0.7.6;
pragma abicoder v2;

// SPDX-License-Identifier: UNLICENSED

import "./Ownable.sol";
import "./ERC20Interface.sol";
import "./IMultiplayerGameDefinition.sol";

//************************************************************************************************
contract MultiplayerGamesManager is Ownable() {
    //************************************************************************************************
    //** Data Structures

    enum ContractState {Running, Paused}
    enum GameRoundState {NotStarted, InProgress, NoMoreBets, Finished}

    struct GameRoundConfig {
        bytes32 roundID;
        address gameAddress;
        bytes32 entropyCommit;
        bytes32 persistentGameDataID;
    }

    struct GameRound {
        GameRoundConfig config;
        GameRoundState state;
        uint256 persistentGameDataPotValueAtEndRound;
        IMultiplayerGameDefinition.Bet[] bets;
        mapping(address => uint256) playerBetCount;
    }

    struct TokenTransferData {
        bytes32 roundID;
        IMultiplayerGameDefinition.Bet[] bets;
    }

    struct PersistentGameData {
        address owner;
        address gameAddress;
        uint256 potValue;
        bytes gameData;
    }

    //************************************************************************************************
    //** Persistent Data

    ContractState public contractState;
    ERC20Interface public gameToken;

    mapping(bytes32 => GameRound) public gameRounds;
    mapping(address => bool) public permittedGameDefinitions;
    mapping(bytes32 => bool) public consumedEntropyCommits; //!TODO this should probably be in a separate contract.
    mapping(bytes32 => PersistentGameData) public persistentGameData;

    uint256 public numRoundsInProgress;

    //************************************************************************************************
    //** Events

    event Constructor(address _gameToken);
    event SetContractState(ContractState _contractState);
    event SetGameDefinitionPermission(address _gameDefinition, bool _permission);
    
    event StartGameRoundIds(bytes32 indexed _roundID, bytes32 indexed _persistentGameDataID);
    event StartGameRound(bytes32 indexed _roundID, bytes32 indexed _persistentGameDataID, GameRoundConfig _config);
    event NoMoreBets(bytes32 indexed _roundID);    
    event EndGameRound(bytes32 indexed _roundID,  bytes32 indexed _persistentGameDataID, bytes32 entropyReveal, address[] _playerAddresses, uint256[] _winAmounts, int256 _persistentGameDataPotWinLoss, bytes _gameResult, bytes _historyToRecord);    
    
    event Bet(bytes32 indexed _roundID, IMultiplayerGameDefinition.Bet _bet);
    event OnTokenTransfer(address _from, uint256 _value, bytes data);
    event WithdrawHouseFunds(address _withdrawalAddress, uint256 _balance, uint256 _tokenBalance);
    event WithdrawERC20Tokens(address _withdrawalAddress, address _tokenAddress, uint256 _tokenBalance);

    //************************************************************************************************
    //** Contract Code

    constructor(address _gameToken) {
        gameToken = ERC20Interface(_gameToken);

        emit Constructor(_gameToken);
    }

    //************************************************************************************************
    function setContractState(ContractState _contractState) public onlyOwner {
        contractState = _contractState;

        emit SetContractState(_contractState);
    }

    //************************************************************************************************
    function setGameDefinitionPermission(address _gameDefinition, bool _permission) public onlyAdmin {
        permittedGameDefinitions[_gameDefinition] = _permission;

        emit SetGameDefinitionPermission(_gameDefinition, _permission);
    }

    //************************************************************************************************
    function createPersistentGameData(bytes32 _ID, address _gameAddress) internal {
        require(persistentGameData[_ID].owner == address(0x0), "Progressive pot already created");
        require(permittedGameDefinitions[_gameAddress], "Game not permitted");

        persistentGameData[_ID].owner = msg.sender;
        persistentGameData[_ID].gameAddress = _gameAddress;
        persistentGameData[_ID].potValue = 0;
        persistentGameData[_ID].gameData = IMultiplayerGameDefinition(gameRounds[_ID].config.gameAddress).getInitialPersistentData();
   }

    //************************************************************************************************
    function getPersistentGameData() public view returns (uint256 _potValue, bytes memory _gameData) {
        _potValue = persistentGameData[bytes32(bytes20(msg.sender))].potValue;
        _gameData = persistentGameData[bytes32(bytes20(msg.sender))].gameData;        
    }

    //************************************************************************************************

    function startGameRoundSplit(bytes32 roundID,
        address gameAddress,
        bytes32 entropyCommit) public ownerAndAdmin {

        GameRoundConfig memory config;
        config.roundID = roundID;
        config.gameAddress = gameAddress;
        config.entropyCommit = entropyCommit;
        config.persistentGameDataID = bytes32(bytes20(msg.sender));
        if(persistentGameData[config.persistentGameDataID].owner == address(0x0)) {
            createPersistentGameData(bytes32(bytes20(msg.sender)), gameAddress);
        }
        return startGameRoundInternal(config);
    }

    //************************************************************************************************
    function startGameRoundInternal(GameRoundConfig memory _config) internal {
        require(contractState == ContractState.Running, "Contract is not running");
        require(gameRounds[_config.roundID].state == GameRoundState.NotStarted, "Round ID already in use");
        require(permittedGameDefinitions[_config.gameAddress], "Game not permitted");
        require(!consumedEntropyCommits[_config.entropyCommit], "Entropy commit already used");
        if (_config.persistentGameDataID != bytes32(0x0)) {
            require(persistentGameData[_config.persistentGameDataID].owner == msg.sender, "Not permitted to use this progressive pot");
            require(persistentGameData[_config.persistentGameDataID].gameAddress == _config.gameAddress, "Incorrect game for progressive pot");
        }

        gameRounds[_config.roundID].config = _config;
        gameRounds[_config.roundID].state = GameRoundState.InProgress;

        consumedEntropyCommits[_config.entropyCommit] = true;
        numRoundsInProgress++;
    
        emit StartGameRoundIds(_config.roundID, _config.persistentGameDataID);
        emit StartGameRound(_config.roundID, _config.persistentGameDataID, _config);
    }

    //************************************************************************************************
    function bet(bytes32 _roundID, IMultiplayerGameDefinition.Bet memory _bet) internal {
        IMultiplayerGameDefinition gameDefinition = IMultiplayerGameDefinition(gameRounds[_roundID].config.gameAddress);

        require(gameRounds[_roundID].state == GameRoundState.InProgress, "Game Round not in progress");
        require(gameRounds[_roundID].bets.length < gameDefinition.maxBetsPerRound(), "Game round is full");
        require(gameRounds[_roundID].playerBetCount[_bet.playerAddress] < gameDefinition.maxBetsPerAddress(), "Too many bets for this address");
        require(_bet.betAmount >= gameDefinition.minBetAmount(), "Bet amount is too small");
        require(_bet.betAmount <= gameDefinition.maxBetAmount(), "Bet amount is too large");
        require(gameDefinition.isBetDataValid(_bet.betData), "Bet data invalid");

        gameRounds[_roundID].bets.push(_bet);
        gameRounds[_roundID].playerBetCount[_bet.playerAddress]++;

        emit Bet(_roundID, _bet);
    }

    //************************************************************************************************
    // Entry point from ERC667 to make a bet
    function onTokenTransfer(address _from, uint256 _value, bytes memory _data) public returns (bool) {
        TokenTransferData memory tokenTransferData = abi.decode(_data, (TokenTransferData));

        // check that the caller is the correct token contract
        require(msg.sender == address(gameToken), "Not the correct Token");

        // make the bet(s)
        uint256 totalDeclaredBet = 0;

        for (uint256 i = 0; i < tokenTransferData.bets.length; i++) {
            require(tokenTransferData.bets[i].playerAddress == _from, "Incorrect player address encoded in bet"); //! TODO.  hm.. think.. could reconstruct this.
            bet(tokenTransferData.roundID, tokenTransferData.bets[i]);
            totalDeclaredBet += tokenTransferData.bets[i].betAmount;
        }

        // check that the bets tally with the abmount of transferred tokens
        require(totalDeclaredBet == _value, "bet amounts do not match transferred token value");

        emit OnTokenTransfer(_from, _value, _data);
        
        return true;
    }

    //************************************************************************************************
    function noMoreBetsForGameRound(bytes32 _roundID) public ownerAndAdmin {
        require(gameRounds[_roundID].state == GameRoundState.InProgress, "Game Round not in progress");

        gameRounds[_roundID].state = GameRoundState.NoMoreBets;

        emit NoMoreBets(_roundID);
    }
    
    //************************************************************************************************
    function endGameRound(bytes32 _roundID, bytes32 _entropyReveal) public ownerAndAdmin {
        require(gameRounds[_roundID].state == GameRoundState.NoMoreBets, "Game Round not in no more bets stage");
        require(keccak256(abi.encodePacked(_entropyReveal)) == gameRounds[_roundID].config.entropyCommit, "Revealed Entropy not valid");

        // call the game contract to determine the results
        (uint256[] memory winAmounts, 
        int256 persistentPotWinLoss, 
        bytes memory newGameData, 
        bytes memory gameResult, 
        bytes memory historyToRecord) = IMultiplayerGameDefinition(gameRounds[_roundID].config.gameAddress).processGameRound(
            gameRounds[_roundID].bets, _entropyReveal, 
            persistentGameData[gameRounds[_roundID].config.persistentGameDataID].potValue, 
            persistentGameData[gameRounds[_roundID].config.persistentGameDataID].gameData);

        persistentGameData[gameRounds[_roundID].config.persistentGameDataID].gameData = newGameData;

        //We need to record the pot value at end time so that future client calls to determine win state know the amount of the pot

        gameRounds[_roundID].persistentGameDataPotValueAtEndRound = persistentGameData[gameRounds[_roundID].config.persistentGameDataID].potValue;

        // set state ahead of payouts to prevent re-entrancy
        gameRounds[_roundID].state = GameRoundState.Finished;
        numRoundsInProgress--;

        // validate and process progressive pots
        if (persistentPotWinLoss < 0) {
            require(int256(persistentGameData[gameRounds[_roundID].config.persistentGameDataID].potValue) - persistentPotWinLoss > 0, "Can't make a progressive pot value go negative");
            persistentGameData[gameRounds[_roundID].config.persistentGameDataID].potValue = persistentGameData[gameRounds[_roundID].config.persistentGameDataID].potValue - uint256(-persistentPotWinLoss);
        } else {
            persistentGameData[gameRounds[_roundID].config.persistentGameDataID].potValue = persistentGameData[gameRounds[_roundID].config.persistentGameDataID].potValue + uint256(persistentPotWinLoss);
        }

        // Payout winnings
        uint256 totalBets = 0;
        uint256 totalPayouts = 0;
        uint256 numBets = gameRounds[_roundID].bets.length;

        address[] memory playerAddresses;
        
        if(gameRounds[_roundID].bets.length != 0) {
            playerAddresses = new address[](gameRounds[_roundID].bets.length);
        }

        for (uint256 i = 0; i < numBets; i++) {
            playerAddresses[i] = gameRounds[_roundID].bets[i].playerAddress;
            require(gameToken.transfer(playerAddresses[i], winAmounts[i]), "Token Transfer Failed");
            totalBets += gameRounds[_roundID].bets[i].betAmount;
            totalPayouts += winAmounts[i];
        }

        // check that the payouts are correct
        // note - should be ok to do this after the payouts due to re-entrancy check above

        require(int256(totalBets) == int256(totalPayouts) + persistentPotWinLoss, "Results don't match bets");

        // Verify that the house has not taken more in than was bet this round

        emit EndGameRound(_roundID, gameRounds[_roundID].config.persistentGameDataID, _entropyReveal, playerAddresses, winAmounts, persistentPotWinLoss, gameResult, historyToRecord); 
    }

    //************************************************************************************************
    
    // function clientCalculateEndGameRound(bytes32 _roundID, bytes32 _entropyReveal) public returns (bytes32 out_roundID, uint256 out_persistentGameDataPotValue, address[] memory out_playerAddresses, uint256[] memory out_winAmounts, int256 out_houseWinLoss, int256 out_persistentGameDataPotWinLoss, bytes memory out_gameResult) {
    //     require((gameRounds[_roundID].state == GameRoundState.NoMoreBets) || (gameRounds[_roundID].state == GameRoundState.Finished), "Game Round neither in progress nor finished");
    //     require(keccak256(abi.encodePacked(_entropyReveal)) == gameRounds[_roundID].config.entropyCommit, "Revealed Entropy not valid");

    //     //Get the progressive pot value to use for calculation

    //     uint256 persistentGameDataPotValue = 0;
    //     bool hasPersistentGameData = (gameRounds[_roundID].config.persistentGameDataID != 0);
        
    //     if (hasPersistentGameData) {
    //         if(gameRounds[_roundID].state == GameRoundState.NoMoreBets) {
    //             //Read directly from the pot
    //             persistentGameDataPotValue = persistentGameData[gameRounds[_roundID].config.persistentGameDataID].potValue;
    //         } else {
    //             //Read from the value stored when endGameRound was called
    //             persistentGameDataPotValue = gameRounds[_roundID].persistentGameDataPotValueAtEndRound;
    //         }
    //     }

    //     // call the game contract to determine the results

    //     (out_winAmounts, out_houseWinLoss, out_persistentGameDataPotWinLoss, out_gameResult, /*ignore historyToRecord*/) = IMultiplayerGameDefinition(gameRounds[_roundID].config.gameAddress).processGameRound(gameRounds[_roundID].bets, _entropyReveal, hasPersistentGameData, persistentGameDataPotValue);
     
    //     // note the address that played

    //     if(gameRounds[_roundID].bets.length != 0) {
    //         out_playerAddresses = new address[](gameRounds[_roundID].bets.length);
    //     }
        
    //     uint256 numBets = gameRounds[_roundID].bets.length;

    //     for (uint256 i = 0; i < numBets; i++) {
    //         out_playerAddresses[i] = gameRounds[_roundID].bets[i].playerAddress;
    //     }
        
    //     //Directly return the results

    //     return (out_roundID, persistentGameDataPotValue, out_playerAddresses, out_winAmounts, out_houseWinLoss, out_persistentGameDataPotWinLoss, out_gameResult); 
    // }

    //************************************************************************************************
    // Needs many additional checks and thoughts on when this should be allowed
    // function withdrawHouseFunds(address payable _withdrawalAddress) public onlyOwner {
    //     require(contractState == ContractState.Paused, "Contract is not paused");
    //     require(numRoundsInProgress == 0, "Rounds still in progress");

    //     uint256 balance = address(this).balance;
    //     uint256 tokenBalance = gameToken.balanceOf(address(this));

    //     _withdrawalAddress.transfer(balance);
    //     require(gameToken.transfer(_withdrawalAddress, tokenBalance), "Game Token transfer failed");

    //     emit WithdrawHouseFunds(_withdrawalAddress, balance, tokenBalance);
    // }

    //************************************************************************************************
    // Needs many additional checks and thoughts on when this should be allowed
    function withdrawERC20Tokens(address payable _withdrawalAddress, address tokenAddress) public onlyOwner {
        require(contractState == ContractState.Paused, "Contract is not paused");
        require(numRoundsInProgress == 0, "Rounds still in progress");

        ERC20Interface token = ERC20Interface(tokenAddress);
        uint256 tokenBalance = token.balanceOf(address(this));

        require(token.transfer(_withdrawalAddress, tokenBalance), "Token transfer failed");

        emit WithdrawERC20Tokens(_withdrawalAddress, tokenAddress, tokenBalance);
    }
}
