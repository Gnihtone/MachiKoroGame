import React, { useState, useCallback, useEffect } from 'react';
import './styles/MainMenu.css';
import MenuSettings from "./MainSettings";
import MainLobbyMenu from "./MainLobbyMenu";
import GameBoard from './GameBoard';

export default function MainMenu({updatePhase, game_state}) {
    function changeToSettings() {
        updatePhase(MenuSettings);
    }

    function goToLobby() {
        updatePhase(MainLobbyMenu);
    }

    return (
        <div id="mainMenuButtons">
            <button onClick={goToLobby}>ИГРАТЬ</button>
            <div></div>
            <button onClick={changeToSettings}>НАСТРОЙКИ</button>
            <div></div>
        </div>
    );
}