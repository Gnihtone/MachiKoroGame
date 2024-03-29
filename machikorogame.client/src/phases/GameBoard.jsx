import React from "react";
import './styles/GameBoard.css';
import MainLobbyMenu from "./MainLobbyMenu";

export default function GameBoard({updatePhase, game_state}) {
    const socket = new WebSocket('ws://127.0.0.1/Game');
    function LeaveGame() {
        updatePhase(MainLobbyMenu);
    }

    return (
        <div id="GameBoard">
            <button id="LeaveGameBtn" onClick={LeaveGame}></button>
        </div>
    )
}