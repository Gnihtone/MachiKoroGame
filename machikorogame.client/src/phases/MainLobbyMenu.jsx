import React from "react";
import './styles/MainLobbyMenu.css';
import MainMenu from "./MainMenu";
import LobbyMenu from './LobbyMenu';

export default function MainLobbyMenu({updatePhase, game_state}) {
    function returnToMainMenu() {
        updatePhase(MainMenu);
    }

    function handleCreationOfLobby(event) {
        event.preventDefault();
        const form = event.target;
        const formData = new FormData(form);
        console.log(Object.fromEntries(formData.entries()));
        updatePhase(LobbyMenu);
    }

    return (
        <div id='MainLobbyMenu'>
            <button className="LeaveMainLobbyMenu" onClick={returnToMainMenu}></button>

            <form method="post" id='CreateLobby' onSubmit={handleCreationOfLobby}>
                <h2>Создание лобби</h2>

                <label htmlFor='lobbyName'>Название лобби: </label>
                <input type='text' name='lobbyName' className="lobbyName"></input>

                <div></div>

                <label htmlFor='lobbyPassword' className="lobbyPassword">Пароль лобби: </label>
                <input type='text' name='lobbyPassword' className="lobbyPassword"></input>
            
                <div></div>

                <label htmlFor="amountOfPlayers">Максимальное число игроков: </label>
                <input type='range' name='amountOfPlayers' min="2" max="4"></input>
            
                
                <button type='submit' id='UnderCreateLobby'>Создать лобби</button>
            </form>

            <div id='JoinLobby'>
                <h2>Присоединение к лобби</h2>
                <button id="UnderJoinLobby">Войти в лобби</button>
            </div>

        </div>
    )
}