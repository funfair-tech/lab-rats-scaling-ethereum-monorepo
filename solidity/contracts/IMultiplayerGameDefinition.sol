pragma solidity >=0.7.6;
pragma abicoder v2;

// SPDX-License-Identifier: UNLICENSED

//************************************************************************************************
interface IMultiplayerGameDefinition {
    struct Bet {
        address playerAddress;
        uint256 betAmount;
        bytes betData;
    }

    function getVersion() external returns (string memory);    

    function maxBetsPerRound() external returns (uint256 _maxBetsPerRound);
    function maxBetsPerAddress() external returns (uint256 _maxBetsPerAddress);
    function minBetAmount() external returns (uint256 _maxBetAmount);
    function maxBetAmount() external returns (uint256 _maxBetAmount);

    function isBetDataValid(bytes memory _betData) external returns (bool);

    function getInitialPersistentData() external returns (bytes memory data);

    function processGameRound(Bet[] memory _bets, bytes32 _entropy, uint256 _persistentPotValue, bytes memory _persistentData) external returns (uint256[] memory _winAmounts, int256 _persistentPotWinLoss, bytes memory _persistentDataOut, bytes memory _gameResult, bytes memory _historyToRecord);
    
}
