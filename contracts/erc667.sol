pragma solidity >=0.8.0;

// SPDX-License-Identifier: UNLICENSED

//************************************************************************************************
interface ITokenTransferReceiver {
    function onTokenTransfer(address _from, uint256 _value, bytes memory _data) external returns (bool);
}

//************************************************************************************************

contract LabRats {
    mapping (address=>uint256) balances;
    uint256 balanceTotal;

    event Transfer(address indexed _from, address indexed _to, uint256 _value);

    constructor() {
        uint256 initialSupply = 100000000000000000000;

        balances[msg.sender] = initialSupply;
        balanceTotal = initialSupply;
    }

    function name() public pure returns (string memory) {
        return "The Lab Rats";
    }

    function symbol() public pure returns (string memory) {
        return "LABRAT";
    }

    function decimals() public pure returns (uint8) {
        return 8;
    }

    function totalSupply() public view returns (uint256) {
        return balanceTotal;
    }

    function balanceOf(address _owner) public view returns (uint256 balance) {
        return balances[_owner];
    }

    function transfer(address _to, uint256 _value) public returns (bool success) {
        require(balances[msg.sender] >= _value, "Insufficient balance");

        balances[msg.sender] -= _value;
        balances[_to] += _value;

        emit Transfer(msg.sender, _to, _value);
        
        return true;
    }

    function transferAndCall(address _to, uint256 _value, bytes memory _data) external returns (bool success) {
        require(transfer(_to, _value), "Token transfer Failed");
    
        require(ITokenTransferReceiver(_to).onTokenTransfer(msg.sender, _value, _data));
        
        return true;    
    }

}
