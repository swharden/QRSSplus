import React from 'react';

class Clock extends React.Component {
    constructor(props) {
        super(props);
        this.state = { date: new Date() };
    }

    componentDidMount() {
        this.timerID = setInterval(() => this.tick(), 1000);
    }

    componentWillUnmount() {
        clearInterval(this.timerID);
    }

    tick() {
        this.setState({ date: new Date() });
    }

    render() {
        return (
            <div className="bg-light border rounded m-5 p-3">
                <div>Current time: <code>{this.state.date.toUTCString()}</code></div>
            </div>
        );
    }
}

export default Clock;