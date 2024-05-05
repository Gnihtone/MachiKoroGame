import React, { useEffect, useState } from "react";
import {useSound} from 'use-sound';
import MainMenu from "./MainMenu";
import victory from '../../public/sounds/victory.mp3'

function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

export default function VictoryMenu({updatePhase, game_state}) {
    return (
        <div>
            <p>Вы победили!</p>
            <button onClick={() => {
                updatePhase(MainMenu);
            }}>Вернуться в главное меню</button>
        </div>
    )
}