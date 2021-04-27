pragma solidity >=0.7.6;
pragma abicoder v2;

// SPDX-License-Identifier: UNLICENSED

import "./IMultiplayerGameDefinition.sol";

/*
    Betting info

    HIGHER = 0,
    LOWER = 1,
    SMALLHIGHER = 2,
    LARGEHIGHER = 3,
    SMALLLOWER = 4,
    LARGELOWER = 5,
*/    

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
        return 100 * 100000000;
    }

    function maxBetAmount() external pure override returns (uint256 _maxBetAmount) {
        return 100 * 100000000;
    }

    function isBetDataValid(bytes memory _betData) external pure override returns (bool) {
        //Bet is simple, a single uint8 with a valid bet option (0 - 5)

        uint8 bet = uint8(_betData[0]);
        if(bet > 5) {
            return false;
        }
        return true;
    }

    function getInitialPersistentData() external pure override returns (bytes memory data) {
        return abi.encode(int256(1000), uint256(0));      
    }

    function processGameRound(Bet[] memory _bets, bytes32 _entropy, uint256 _persistentPotValue, bytes memory _persistentData) external pure override returns (uint256[] memory _winAmounts, int256 _persistentPotWinLoss, bytes memory _persistentDataOut, bytes memory _gameResult, bytes memory _historyToRecord) {
        
       
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
        
        // //Work out how big the pot it and split it

        (_persistentPotWinLoss, _winAmounts) = calculateRemainingPotAndWinnings(movement, _persistentPotValue, _bets);

    }

    function calculateRemainingPotAndWinnings(int256 _movement, uint256 _persistentPotValue, Bet[] memory _bets) internal pure returns (int _persistentPotWinLoss, uint256[] memory _winAmounts) {
        
         //Prepare the wins for each bet

        _winAmounts = new uint256[](_bets.length);

        uint256 totalPot = _persistentPotValue;
        uint256 winSplitCount = 0;
        uint256 amountPerSplit = 0;

        //Go through the bets to decide on wins and allocations

        for(uint i = 0; i < _bets.length; i++) {
            totalPot += _bets[i].betAmount;
            uint8 betType = uint8(_bets[i].betData[0]);
            if (_movement < 0) {
                if (betType == 1) {
                    _winAmounts[i] = 1;
                } else if ((_movement < -5) && (betType == 5)) {
                    _winAmounts[i] = 2;
                } else if ((_movement >= -5) && (betType == 4)) {
                    _winAmounts[i] = 2;
                }
            } else if (_movement > 0) {
                if (betType == 0) {
                    _winAmounts[i] = 1;
                } else if ((_movement > 5) && (betType == 3)) {
                    _winAmounts[i] = 2;
                } else if ((_movement <= 5) && (betType == 2)) {
                    _winAmounts[i] = 2;
                }
            }

            winSplitCount += _winAmounts[i];
        }

        //Allocate the winning prize from the pot, without rounding

        if(winSplitCount != 0) {
            amountPerSplit = totalPot / winSplitCount;

            //Pay out as needed

            for(uint i = 0; i < _bets.length; i++) {
                _winAmounts[i] = _winAmounts[i] * amountPerSplit;
                totalPot -= _winAmounts[i];
            }
        }

        //Calculate the retained difference

        _persistentPotWinLoss = int256(totalPot) - int256(_persistentPotValue);
    }
}
