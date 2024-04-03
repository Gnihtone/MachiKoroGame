import React from "react";
import './styles/MainLobbyMenu.css';
import MainMenu from "./MainMenu";
import LobbyMenu from './LobbyMenu';

export default function MainLobbyMenu({updatePhase, game_state}) {
    const user_id = game_state.state.user_id;

    function returnToMainMenu() {
        updatePhase(MainMenu);
    }

    async function handleCreationOfLobby(event) {
        event.preventDefault();
        const form = event.target;
        const formData = new FormData(form);
        var entries = Object.fromEntries(formData.entries());
        console.log(entries);
        var requestOptions = {
            mode: 'cors',
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ 
                name: entries.lobbyName,
                max_players: parseInt(entries.amountOfPlayers),
                password: entries.lobbyPassword
            })
        };
        var data = await (await fetch('http://localhost:5044/api/lobby/create', requestOptions)).json()
        console.log(data);
        var lobby_id = data['id'];

        const user_id2 = (await (await fetch(`http://localhost:5044/api/user/get?id=${user_id}`)).json())['id'];
        console.log(user_id2);

        requestOptions = {
            mode: 'cors',
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ 
                user_id: user_id2,
                lobby_id: lobby_id,
                password: entries.lobbyPassword
            })
        };
        await fetch('http://localhost:5044/api/user/updatelobby', requestOptions)
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