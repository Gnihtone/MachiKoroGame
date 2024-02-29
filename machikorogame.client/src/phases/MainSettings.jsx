import MainMenu from './MainMenu';
import './styles/MainSettings.css';

export default function MenuSettings({updatePhase, game_state}) {
    function returnToMainMenu() {
        updatePhase(MainMenu);
    }

    function onVolumeUpdate(event) {
        game_state.setState({...game_state.state, volume: event.target.value});
        console.log("Updated state: ", game_state.state);
    }

    function onMusicUpdate(event) {
        game_state.setState({...game_state.state, music: event.target.value});
        console.log("Updated state: ", game_state.state);
    }

    return (
        <div id='MainSettings'>
            <div className='paramname'>Звуки:</div>
            <input type='range' id='volume' name='volume' min={0} max={100} value={game_state.state.volume} onChange={onVolumeUpdate} />
            <div></div>

            <div className='paramname'>Музыка: </div>
            <input type='range' id='music' name='music' min={0} max={100} value={game_state.state.music} onChange={onMusicUpdate} />
            <div></div>

            <button onClick={returnToMainMenu}>Назад</button>
        </div>
    );
}