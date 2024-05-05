import React from 'react';
import introAnimationData from '../assets/First intro.json';
import Lottie, { LottieRefCurrentProps } from 'lottie-react';
import {useRef} from 'react'
import MainMenu from './MainMenu';
import RejoinLobby from './RejoinLobby';


export default function onOpenPage({ updatePhase }) {
    const introRef = useRef<LottieRefCurrentProps>(null);

    return (
        <div>
            <Lottie 
                animationData={introAnimationData}
                loop={false}
                lottieRef={introRef}
                onComplete={
                    () => {
                        introRef.current?.destroy();
                        updatePhase(RejoinLobby);
                    }
                }
            />
        </div>
    );
}