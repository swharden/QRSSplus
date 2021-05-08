import React from 'react';

class Dashboard extends React.Component {
    constructor(props) {
        super(props);
        this.state = { grabbersJson: "update needed" };
    }

    update(data) {
        let grabberCount = Object.keys(data.grabbers).length;
        this.setState({ grabbersJson: `Found ${grabberCount} active grabbers` });
    }

    render() {
        return (
            <div>
                <h1>Dashboard</h1>
                <div><code><pre>{this.state.grabbersJson}</pre></code></div>
            </div>
        );
    }
}

export default Dashboard;