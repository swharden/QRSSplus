import React from 'react';
import Clock from './Clock';
import Dashboard from './Dashboard';

class App extends React.Component {
  constructor(props) {
    super(props);
    this.state = { grabbersJson: "update needed" };
    this.dashboardRef = React.createRef();
  }

  updateGrabbers() {
    this.setState({ grabbersJson: "updating..." });
    fetch('https://qrssplus.z20.web.core.windows.net/grabbers.json')
      .then(response => response.json())
      .then(data => this.gotUpdatedJson(data));
  }

  gotUpdatedJson(data) {
    this.dashboardRef.current.update(data);
  }


  render() {
    return (
      <div className="container">
        <h1>QRSS Plus</h1>
        <h3>Automatically Updating Active QRSS Grabbers List</h3>
        <Clock />
        <button onClick={this.updateGrabbers.bind(this)}>update</button>
        <pre>{this.state.grabbersJson}</pre>
        <Dashboard ref={this.dashboardRef} />
      </div>
    );
  }
}

export default App;