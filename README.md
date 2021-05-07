# Lab Rats: Eth Global Scaling Ethereum Hackathon

This is the labs rats scaling hackathon project

A multiplayer trading graph betting game where you bet if the graph will go up or down. Sign and send the transaction on every bet using optimism L2 to show how you can do complete on chain betting without spending loads of ETH on gas.

[Play](https://lab-rats-scaling-ethereum.netlify.app)


| What | Status |
| --- | :-- |
| Client | [![Client](https://api.netlify.com/api/v1/badges/0d5cf2c3-b3d6-43b6-805b-265ff6f3cfed/deploy-status)](https://app.netlify.com/sites/lab-rats-scaling-ethereum/deploys) |
| Contracts | [![Contracts](https://github.com/funfair-tech/lab-rats-scaling-ethereum-monorepo/actions/workflows/deploy-contracts.yml/badge.svg)](https://github.com/funfair-tech/lab-rats-scaling-ethereum-monorepo/actions/workflows/deploy-contracts.yml) |
| Server | [![Server](https://teamcity.funfair.io/app/rest/builds/buildType:Labs_EthGlobalScalingEthereum_Server_Master/statusIcon.svg)](https://teamcity.funfair.io/viewType.html?buildTypeId=Labs_EthGlobalScalingEthereum_Server_Master&branch_Labs_EthGlobalScalingEthereum_Server=%3Cdefault%3E&tab=buildTypeStatusDiv) |
| DB | [![SQL](https://github.com/funfair-tech/lab-rats-scaling-ethereum-monorepo/actions/workflows/reformat-sql.yml/badge.svg)](https://github.com/funfair-tech/lab-rats-scaling-ethereum-monorepo/actions/workflows/reformat-sql.yml) |

## Description

RatTrace is a simple multi-player graph trading game where players can bet against whether the price is going to go higher or lower.  

Games are started each by the server issuing a ``start game`` transaction, and when the players see the chain event for this they can place their bets by placing ``bet`` transactions. After a period of time the server issues a ``stop betting`` transaction followed by an ``end game`` transaction where the results of the game are reported in an event and displayed in the client.

<insert screenshot of sequence diagram here>


## How its made

### Contracts

Built using hardhat

#### Dependencies

* Hardhat

### Dapp Client

Built using react and ethers....

#### Dependencies

* Funfair Wallet
* ethersjs

### Server

Built using C# using dotnet core 5 with a MS SQL Server Database.





#### Dependencies
* Funfair Ethereum Services (JSON-RPC/Web3 wrapper)
* SQL Server