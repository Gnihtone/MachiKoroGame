import React from "react";
import './styles/LobbyMenu.css';
import MainLobbyMenu from "./MainLobbyMenu";
import GameBoard from "./GameBoard";

export default function LobbyMenu({updatePhase, game_state}) {
    function LeaveLobby() {
        updatePhase(MainLobbyMenu);
    }

    function StartGame() {
        updatePhase(GameBoard);
    }

    return (
        <div id="LobbyMenu">
            <div className="goodstuff">Название лобби</div>
            <div id="buttons">
                <button id="StartGameBtn" className="LobbyBtn" onClick={StartGame}>Запустить игру</button>
                <div></div>
                <button className="LobbyBtn" onClick={LeaveLobby}>Покинуть лобби</button>
            </div>
            <div id="Players">

            </div>
        </div>
    )
}