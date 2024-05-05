import React, { useEffect, useState } from "react";
import MainMenu from "./MainMenu";
import './styles/RejoinLobby.css';
import GameBoard from "./GameBoard";


export default function RejoinLobby({updatePhase, game_state}) {
    function goToMain() {
        updatePhase(MainMenu);
    }

    function goToGame() { 
        updatePhase(GameBoard);
    }

    const user_id4 = game_state.state.user_id;
    var playerData = null;

    async function fet() {
        const data = (await (await fetch(`http://localhost:5044/api/user/get?id=${user_id4}`)).json())
        playerData = data['currentLobbyId'];
    }

    fet().then(() => {
        if (playerData == null) {
            goToMain();
        }
    });

    return (
        <div>
            <p>У вас имеется покинутая игра, желаете присоединиться?</p>
            <button className="answer" onClick={goToGame}>Да</button>
            <button className="answer" onClick={goToMain}>Нет</button>
        </div>
    )
}