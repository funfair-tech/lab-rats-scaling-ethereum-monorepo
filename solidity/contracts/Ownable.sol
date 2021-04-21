pragma solidity ^0.4.24;

/**
 * @title Ownable
 * @dev The Ownable contract has an owner address, and provides basic authorization control
 * functions, this simplifies the implementation of "user permissions".
 */
contract Ownable {
    address public owner;
    mapping(address => bool) admins;

    address newOwner;
    
    /**
     * @dev The Ownable constructor sets the original `owner` of the contract to the sender
     * account.
     */
    constructor() public {
        owner = msg.sender;
    }


    /**
     * @dev Throws if called by any account other than the owner.
     */
    modifier onlyOwner() {
        require(msg.sender == owner, "Sender is not owner");
        _;
    }

    modifier ownerAndAdmin() {
        require(msg.sender == owner || admins[msg.sender], "Sender is not owner or admin");
        _;
    }

    modifier onlyAdmin() {
        require(admins[msg.sender], "Sender is not an admin");
        _;
    }

    /**
     * @dev Allows the current owner to transfer control of the contract to a newOwner.
     * @param _newOwner The address to transfer ownership to.
     */
    function transferOwnership(address _newOwner) public onlyOwner {
        if (_newOwner != address(0)) {
            newOwner = _newOwner;
      }
    }

    function acceptOwnership() public {
        if (msg.sender == newOwner) {
            owner = newOwner;
      }
    }

    // assign admin (access to fund contract and airdrop)
    function setAdmin(address admin_address, bool isAdmin) public onlyOwner {
        admins[admin_address] = isAdmin;
    }

}