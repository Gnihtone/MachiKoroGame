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

export default function GameBoard({updatePhase, game_state}) {
    var socket = null;
    const [isConnected, setIsConnected] = useState(false);
    
    var userId = null;

    const [yourData, setYourData] = useState(null);
    const [otherPlayerData, setOtherPlayerData] = useState(null);
    const [playerCards, setPlayerCards] = useState([]);
    const [availvableCards, setAvailableCards] = useState([]);
    const [currentMoney, setCurrentMoney] = useState(0);

    const LeaveGame = useRef(null);
    

    useEffect(() => {
        if (!isConnected) {
            async function getUserId() {
                const data = await ((await fetch(`http://localhost:5044/api/user/get?id=${game_state.state.user_id}`)).json());
                console.log(data['id']);
                return data['id'];
            }
    
            if (userId == null) {
                getUserId().then((value) => {userId = value;}); // Держится на вере
            }

            socket = new WebSocket('ws://127.0.0.1/Game');

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
                    var players = data['Players']
                    var tmp_players = []
                    setAvailableCards(data['AvailableBuildings']);
                    players.forEach(player => {
                        if (player['Id'] == userId) {
                            console.log(`Me:`);
                            console.log(player);
                            setYourData(player);
                            setPlayerCards(player['Cards']);
                            setCurrentMoney(player['Money']);
                        } else {
                            console.log(`Other:`);
                            console.log(player);
                            tmp_players.push(player);
                        }
                    });
                    setOtherPlayerData(tmp_players);
                }
            }

            setIsConnected(true);
        }

        return LeaveGame;
    }, [socket, setIsConnected, setOtherPlayerData, setYourData, userId, socket, setAvailableCards]);

    return (
        <div id="GameBoard">
            <button id="LeaveGameBtn" onClick={LeaveGame.current}></button>

            <div id="GamePlace">
                {
                    Object.keys(availvableCards).map((value, idx) => {
                        return (
                            <button className="card">
                                <img className="card" src={"cards/" + value + ".png"}></img>
                                <div className="amount">
                                    <div className="cardBackground"></div>
                                    <p className="card">{availvableCards[value]}</p>
                                </div>
                            </button>
                        )
                    })
                }
            </div>

            <div id="PlayerPlace">
                
                <div id="Money">
                    <img src="cards/coin.png"></img>
                    <p>: {currentMoney}</p>
                </div>
                <div id="PlayerCards">
                    {
                        Object.keys(playerCards).map((value, idx) => {
                            return (
                                <div className="card">
                                    <img className="card" src={"cards/" + value + ".png"}></img>
                                    <div className="amount">
                                        <div className="cardBackground"></div>
                                        <p className="card">{playerCards[value]}</p>
                                    </div>
                                </div>
                            )
                        })
                    }
                </div>
            </div>

            
        </div>
    )
}