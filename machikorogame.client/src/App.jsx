import React, { useCallback, useRef } from 'react'
import { useEffect, useState } from 'react';
import './App.css';
import onOpenPage from './phases/OpenPage';
import mainMenu from './phases/MainMenu';
import { useCookies } from 'react-cookie';
import { v4 as uuidv4 } from 'uuid';
import { instanceOf } from 'prop-types';
import { withCookies, Cookies } from 'react-cookie';
import MainMenu from './phases/MainMenu';

function Phases({ phaseRef, updatePhase, game_state }) {
    return phaseRef({updatePhase, game_state});
}

class App extends React.Component {
    static propTypes = {
        cookies: instanceOf(Cookies).isRequired,
      };

    constructor(props) {
        super(props);
        
        const { cookies } = props;

        this.state = {
            user_id: cookies.get('user_id') || null,
            volume: cookies.get('volume') || 50,
            music: cookies.get('music') || 50,
            // phaseRef: onOpenPage
            phaseRef: MainMenu
        };

        this.updatePhase = this.updatePhase.bind(this);
        this.updateCookies = this.updateCookies.bind(this);
    }

    updatePhase(newPhase) {
        this.updateCookies();
        this.setState({
            ...this.state, 
            phaseRef: newPhase
        });
    }

    updateCookies() {
        const { cookies } = this.props;

        cookies.set('volume', this.state.volume, {path: '/'});
        cookies.set('music', this.state.music, {path: '/'});
    }

    render() {
        if (this.state['user_id'] === null) {
            this.state['user_id'] = uuidv4();
            const { cookies } = this.props;

            cookies.set('user_id', this.state['user_id'], {path: '/'})
        }

        return (
            <Phases phaseRef={this.state.phaseRef} updatePhase={this.updatePhase} game_state={this} />
        );
    }
}

export default withCookies(App);
