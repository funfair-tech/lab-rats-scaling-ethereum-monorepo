pragma solidity ^0.4.24;

import "./ERC20Interface.sol";
import "./Ownable.sol";

contract Faucet is Ownable{
    ERC20Interface _token;

    event DistributedEth(address from, address to, uint256 amount);
    event DistributedToken(address from, address to, uint256 amount);

    event SentFundsToContract(address from, address to, uint256 amount);

    event WithdrewEthFromContract(address to, uint256 amount);
    event WithdrewTokenFromContract(address to, uint256 amount);

    constructor(address token) public {
        _token = ERC20Interface(token);
    }

    function withdrawEth(address to) public onlyOwner {
        uint256 totalEth = address(this).balance;
        to.transfer(totalEth);
        emit WithdrewEthFromContract(to, totalEth);
    }

    function withdrawToken(address to) public onlyOwner {
        uint256 totalToken = _token.balanceOf(address(this));
        require(_token.transfer(to, totalToken), "Failed to withdraw token");
        emit WithdrewTokenFromContract(to, totalToken);
    }

    // this is called when Eth is transferred to the contract address
    function () public payable {
        require(msg.value > 0, "Must transfer an actual amount of ETH");
        emit SentFundsToContract(msg.sender, address(this), msg.value);
    }

    function distributeEth(address recipient, uint256 value) public onlyAdmin {
        require(value <= address(this).balance, "Can't withdraw more ETH than faucet has");
        recipient.transfer(value);
        emit DistributedEth(address(this), recipient, value);
    }

    function distributeToken(address recipient, uint256 value) public onlyAdmin {
        require(value <= _token.balanceOf(address(this)), "Can't withdraw more tokens than faucet has");
        require(_token.transfer(recipient, value), "Failed to distribute token");
        emit DistributedToken(address(this), recipient, value);
    }

    function distributeTokenAndEth(address recipient, uint256 ethValue, uint256 tokenValue) public onlyAdmin {

        distributeEth(recipient, ethValue);
        distributeToken(recipient, tokenValue);
    }
}