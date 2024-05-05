import React, { useEffect, useState } from "react";
import {useSound} from 'use-sound';
import MainMenu from "./MainMenu";
import lose from '../../public/sounds/lose.mp3'

export default function LoseMenu({updatePhase, game_state}) {
    // const [lost] = useSound(lose);
    // lost();

    return (
        <div>
            <p>Вы проиграли</p>
            <button onClick={() => {
                updatePhase(MainMenu);
            }}>Вернуться в главное меню</button>
        </div>
    )
}