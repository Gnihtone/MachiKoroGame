import React, { useEffect, useState, useRef } from "react";
import './styles/GameBoard.css';
import MainLobbyMenu from "./MainLobbyMenu";

function sendMessage(socket, type, message, game_state) {
    const user_id = game_state.state.user_id;
    var msg = {
        "id": user_id, 
        "type": type,
        "message": message
    }
    console.log(JSON.stringify(msg));
    socket.send(JSON.stringify(msg));
}

function Roll1({socket, game_state}) {
    function onPress() {
        sendMessage(socket, "roll", "1", game_state);
    }

    return (
        <>
            <button className="Roll1" onClick={onPress}>
                <p>Roll 1 dice</p>
            </button>
        </>
    )
}

function Roll2({socket, game_state}) {
    function onPress() {
        sendMessage(socket, "roll", "2", game_state);
    }
    return (
        <>
            <button className="Roll2" onClick={onPress}>
                <p>Roll 2 dices</p>
            </button>
        </>
    )
}

function Continue({socket, game_state}) {
    function onPress() {
        sendMessage(socket, "continue", "", game_state);
    }

    return (
        <>
            <button className="Continue" onClick={onPress}>
                <p>Continue</p>
            </button>
        </>
    )
}

function Continue2({socket, game_state}) {
    function onPress() {
        sendMessage(socket, "continue", "", game_state);
    }

    return (
        <>
            <button className="Continue2" onClick={onPress}>
                <p>Continue</p>
            </button>
        </>
    )
}

function BuildableCard({cardType, amount, socket, game_state}) {
    function onPress() {
        sendMessage(socket, "build", cardType, game_state);
    }

    return (
        <button className="card" onClick={onPress}>
            <img className="card" src={"cards/" + cardType + ".png"}></img>
            <div className="amount">
                <div className="cardBackground"></div>
                <p className="card">{amount}</p>
            </div>
        </button>
    )
}

function BuildableSight({sightType, have, socket, game_state}) {
    function onPress() {
        if (have) {
            return;
        }
        sendMessage(socket, "build", sightType, game_state);
    }

    
    return (
        <button className="card" onClick={onPress}>
            <img className="card" src={"cards/" + sightType + (have ? 2 : 1) + ".png"}></img>
        </button>
    )
}

function GameStage({stage, data, yourMove, socket, game_state}) {
    if (yourMove) {
        if (stage == 0) {
            if (!data['canRoll']) {
                return (
                    <Continue socket={socket} game_state={game_state}></Continue>
                )
            }
            else if (data['canRollTwo']) {
                return (
                    <>
                        <Roll1 socket={socket} game_state={game_state}></Roll1>
                        <Roll2 socket={socket} game_state={game_state}></Roll2>
                        <Continue socket={socket} game_state={game_state}></Continue>
                    </>
                )
            } else {
                return (
                    <>
                        <Roll1 socket={socket} game_state={game_state}></Roll1>
                        <Continue socket={socket} game_state={game_state}></Continue>
                    </>
                )
            }
        }
        else if (stage == 1) {
            return (
                <>
                    {Object.keys(data).map((value, idx) => {
                        return (
                            <>
                                <BuildableCard cardType={value} amount={data[value]} socket={socket} game_state={game_state}></BuildableCard>
                            </>
                        )
                    })}
                    <Continue2 socket={socket} game_state={game_state}></Continue2>
                </>
            )
        }
    } else {
        return (
            <p className="waiting">Waiting for another player</p>
        )
    }
}


export default function GameBoard({updatePhase, game_state}) {
    var socket = null;
    const socket2 = useRef(null);
    const [isConnected, setIsConnected] = useState(false);
    
    var userId = "";
    const [userId2, setUserId] = useState("");

    const [yourData, setYourData] = useState(null);
    const [playerData, setPlayerData] = useState([]);
    const [chosenPlayer, setChosenPlayer] = useState({'Sights': {}, 'Cards': {}});
    const [availvableCards, setAvailableCards] = useState([]);
    const [currentMoney, setCurrentMoney] = useState(0);
    const [currentPlayer, setCurrentPlayer] = useState("");
    const [currentStage, setCurrentStage] = useState(1);
    const [lastRoll, setLastRoll] = useState(0);
    const [canRoll, setCanRoll] = useState(true);

    const LeaveGame = useRef(null);

    useEffect(() => {
        if (!isConnected) {
            async function getUserId() {
                const data = await ((await fetch(`http://localhost:5044/api/user/get?id=${game_state.state.user_id}`)).json());
                console.log(data['id']);
                return data['id'];
            }

            async function mainfunc() {
                if (userId == "") {
                    userId = await getUserId();
                }

                socket = new WebSocket('ws://127.0.0.1/Game');
                setIsConnected(true);

                setUserId(userId);

                LeaveGame.current = () => {
                    socket.close(1000, "Player has left lobby");
                    updatePhase(MainLobbyMenu);
                }

                socket.onopen = (event) => {
                    console.log("connected");
                }
            
                socket.onmessage = async (event) => {
                    console.log(`Got message: ${event.data}`);
                    var msg = JSON.parse(event.data);
                    if (msg['type'] == 'bad') {
                        LeaveGame();
                        return;
                    } else if (msg['type'] == 'wrong') {
                        console.warn('something went wrong');
                    } else if (msg['type'] == 'hello') {
                        sendMessage(socket, "hello", "", game_state);
                    } else if (msg['type'] == 'maininfo') {
                        var data = JSON.parse(msg['message']);
                        var players = data['Players'];
                        setAvailableCards(data['AvailableBuildings']);
                        setPlayerData(players);
                        console.log(data['AvailableBuildings']);
                        players.forEach(player => {
                            if (player['Id'] == userId) {
                                console.log(`Me:`);
                                console.log(player);
                                setYourData(player);
                                setChosenPlayer(player);
                                console.log(player);
                                setCurrentMoney(player['Money']);
                            } else {
                                console.log(`Other:`);
                                console.log(player);
                            }
                        });
                        setCurrentPlayer(data['CurrentPlayerId']);
                        setCurrentStage(data['CurrentMoveType']);
                    } else if (msg['type'] == 'roll') {
                        setLastRoll(msg['message']);
                    }
                }

                socket2.current = socket;
            }

            mainfunc();
        }

        return LeaveGame;
    }, [socket, setIsConnected, 
        setPlayerData, setYourData, userId, socket, 
        setChosenPlayer, setAvailableCards, setUserId, setCurrentPlayer]);

    function changePlayer(player) {
        setCurrentMoney(player['Money']);
        setChosenPlayer(player);
    }

    function isYourMove() {
        return userId2 == currentPlayer;
    }

    return (
        <div id="GameBoard">
            <p id="MovingPlayerId">Moving player: {currentPlayer.slice(0, 7) + '...'}</p>

            <p id="YourId">Your id: {userId2.slice(0, 7) + '...'}</p>

            <button id="LeaveGameBtn" onClick={LeaveGame.current}></button>

            <div id="GamePlace">
                <GameStage stage={currentStage} data={(currentStage == 1 ? availvableCards : 
                    {
                        "canRoll": canRoll,
                        "canRollTwo": true,
                    }
                    )} yourMove={isYourMove()} socket={socket2.current} game_state={game_state}></GameStage>
            </div>

            <div id="PlayerPlace">
                
                <div id="Money">
                    <img src="cards/coin.png"></img>
                    <p>: {currentMoney}</p>
                </div>
                <div id="PlayerCards">
                    {
                        Object.keys(chosenPlayer['Cards']).map((value, idx) => {
                            return (
                                <button key={idx} className="card">
                                    <img className="card" src={"cards/" + value + ".png"}></img>
                                    <div className="amount">
                                        <div className="cardBackground"></div>
                                        <p className="card">{chosenPlayer['Cards'][value]}</p>
                                    </div>
                                </button>
                            )
                        })
                    }
                </div>
                <div id="PlayerSights">
                    {
                        Object.keys(chosenPlayer['Sights']).map((value, idx) => {
                            return <BuildableSight sightType={value} have={chosenPlayer['Sights'][value]} socket={socket2.current} game_state={game_state}></BuildableSight>
                        })
                    }
                </div>
            </div>

            <p id="LastRoll">{(isYourMove() ? "Your" : "Player")} last roll: {lastRoll}</p>

            <div id="ChangePlayer">
                {
                    playerData.map((value, idx) => {
                        return (
                            <div className="rad1">
                                <input type='radio' className="playerCheck" checked={chosenPlayer == value} onChange={() => changePlayer(value)}></input>
                                <label>{value['Id'].slice(0, 7) + '...'}</label>
                            </div>
                        )
                    })
                }
            </div>
        </div>
    )
}