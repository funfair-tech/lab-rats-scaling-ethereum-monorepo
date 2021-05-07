# Lab Rats: Eth Global Scaling Ethereum Hackathon

This is RatTrace, the lab rats scaling hackathon project

A multiplayer trading simulation game running in real-time on the Optimism chain. Players try to predict where the price will go next by placing tokens on the graph. This proof of concept shows that all token transfers and game logic can be performed on-chain without spending excessive ETH on gas costs.

[Play](https://lab-rats-scaling-ethereum.netlify.app)


| What | Status |
| --- | :-- |
| Client | [![Client](https://api.netlify.com/api/v1/badges/0d5cf2c3-b3d6-43b6-805b-265ff6f3cfed/deploy-status)](https://app.netlify.com/sites/lab-rats-scaling-ethereum/deploys) |
| Contracts | [![Contracts](https://github.com/funfair-tech/lab-rats-scaling-ethereum-monorepo/actions/workflows/deploy-contracts.yml/badge.svg)](https://github.com/funfair-tech/lab-rats-scaling-ethereum-monorepo/actions/workflows/deploy-contracts.yml) |
| Server | [![Server](https://teamcity.funfair.io/app/rest/builds/buildType:Labs_EthGlobalScalingEthereum_Server_Master/statusIcon.svg)](https://teamcity.funfair.io/viewType.html?buildTypeId=Labs_EthGlobalScalingEthereum_Server_Master&branch_Labs_EthGlobalScalingEthereum_Server=%3Cdefault%3E&tab=buildTypeStatusDiv) |
| DB | [![SQL](https://github.com/funfair-tech/lab-rats-scaling-ethereum-monorepo/actions/workflows/reformat-sql.yml/badge.svg)](https://github.com/funfair-tech/lab-rats-scaling-ethereum-monorepo/actions/workflows/reformat-sql.yml) |

## Description

RatTrace is a simple multi-player graph trading game where players can bet against whether the price is going to go higher or lower.  

Games are started each by the server issuing a ``start game`` transaction, and when the players see the chain event for this they can place their bets by placing ``bet`` transactions. After a period of time the server issues a ``stop betting`` transaction followed by an ``resolve game`` transaction where the results of the game are reported in an event and displayed in the client.

Random numbers for the game are provided by a commit-reveal scheme.  The server generates a large random number 'Reveal Seed' and hashes that to produce the 'Commit seed '.  The commit seed is passed in the ``start game`` transaction, and the reveal seed passed in the ``resolve game`` transaction, and verified against the commit seed before it is used.

![Sequence Diagram](images/RatTraceContractFlow.png)

The server includes a faucet to issue test funds so that players can play without needing to go and get any from anywhere else.

## How its made

RatTrace was built iteratively with all parts being developed in parallel.

### Contracts

There are several contracts

* 'ERC667' token - the LABRATS token that bets on the game are made in
* 'Faucet' contract - for issuing the funds to players so they can play.
* 'MultiplayerGamesManager' - controls the game betting cycle
* 'RatTrace' game contract - the game logic

#### Dependencies

* Hardhat

### Dapp Client

The client is made up of a web app hosting a game component.

The web app is built in React and all view components were created and styled from scratch. We used the FunFair Wallet for authentication and all web3 functionality and React Redux to manage state. Player count and game history are received through web sockets which we connect to via signalR and there is an integrated http faucet which we call using the javascript fetch library. All other state communication is via JSON-RPC logs.  

The game display has been built from scratch using the following external components:

* Funfair game engine (FFEngine module) - Our in-house developed game engine which extends the popular THREE.js WebGL 3D renderer
* A small selection of previously made assets such as fonts and images in the client/public/game folder

#### Dependencies
* Funfair Wallet
* ethersjs

### Server

Built using C# using dotnet core 5 with a MS SQL Server Database.  This 

#### Dependencies
* Funfair Ethereum Services (JSON-RPC/Web3 wrapper)
* SQL Server

