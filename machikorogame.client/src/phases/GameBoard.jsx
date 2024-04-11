import React from "react";
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
    const socket = new WebSocket('ws://127.0.0.1/Game');

    socket.onopen = (event) => {
        console.log("connected");
    }

    socket.onmessage = (event) => {
        console.log(`Got message: ${event.data}`);
        var msg = JSON.parse(event.data);
        if (msg['type'] == 'bad') {
            LeaveGame();
            return;
        } else if (msg['type'] == 'hello') {
            sendMessage(socket, "hello", "", game_state);
        } else if (msg['type'] == 'start') {
            socket.close(1000, "Game started");
            updatePhase(GameBoard);
        }
    }

    function LeaveGame() {
        socket.close(1000, "Player leaved");
        updatePhase(MainLobbyMenu);
    }

    return (
        <div id="GameBoard">
            <button id="LeaveGameBtn" onClick={LeaveGame}></button>
        </div>
    )
}