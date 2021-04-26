pragma solidity >=0.7.6;
pragma abicoder v2;

// SPDX-License-Identifier: UNLICENSED

import "./IMultiplayerGameDefinition.sol";

//************************************************************************************************
contract RatTrace is IMultiplayerGameDefinition {
    
    function getVersion() external pure override returns (string memory) {
        return 'v 0.00 - first pass';
    }

    function maxBetsPerRound() external pure override returns (uint256 _maxBetsPerRound) {
        return 100;
    }

    function maxBetsPerAddress() external pure override returns (uint256 _maxBetsPerAddress) {
        return 1;
    }

    function minBetAmount() external pure override returns (uint256 _maxBetAmount) {
        return 100;
    }

    function maxBetAmount() external pure override returns (uint256 _maxBetAmount) {
        return 100;
    }

    function isBetDataValid(bytes memory /*_betData*/) external pure override returns (bool) {
        
        //Todo - validate the bet
        return true;
    }

    function getInitialPersistentData() external pure override returns (bytes memory data) {
        return abi.encode(int256(1000), uint256(0));      
    }

    function processGameRound(Bet[] memory _bets, bytes32 _entropy, uint256 _persistentPotValue, bytes memory _persistentData) external pure override returns (uint256[] memory _winAmounts, int256 _persistentPotWinLoss, bytes memory _persistentDataOut, bytes memory _gameResult, bytes memory _historyToRecord) {
        
        //Prepare the wins for each bet

        _winAmounts = new uint[](_bets.length);

        //Decide on the movement and record it

        int256 movement = int256(uint256(_entropy) % 22) - 10;

        (int256 currentValue, uint256 history) = abi.decode(_persistentData, (int256, uint256));
        history = history << 8;
        if(movement < 0) {
            history |= uint256(256 + movement);
        } else {
            history |= uint256(movement);
        }
        
        _gameResult = abi.encode(currentValue, movement, history);
        currentValue += movement;
        _historyToRecord = abi.encode(currentValue);
        _persistentDataOut = abi.encode(currentValue, history);
        
        //Go through the bets to decide on wins and allocations

        //Allocate the winning prize for the bet and persistent data

        //Pay out as needed

        //Reduce the winning pool

    }
}
