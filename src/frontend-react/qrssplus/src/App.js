import React from 'react';

class App extends React.Component {

  constructor(props) {
    super(props);
    this.state = {
      grabbersJson: {},
      grabbers: {}
    };
  }

  onUpdateGrabbers() {
    this.setState({ grabbersJson: "downloading..." });
    fetch('https://qrssplus.z20.web.core.windows.net/grabbers.json')
      .then(response => response.json())
      .then(obj => {
        this.setState({ grabbersJson: obj });
        this.setState({ grabbers: obj.grabbers });
      });
  }

  render() {
    return (
      <div className="container">
        <h1>QRSS Plus</h1>
        <h3>Automatically Updating Active QRSS Grabbers List</h3>

        <div className="bg-dark text-light shadow border rounded m-5 p-3">
          <h1>Component Test</h1>
          <button onClick={this.onUpdateGrabbers.bind(this)}>update</button>

          <div>
            <ul>
              {
                Object.keys(this.state.grabbers).map(id => (
                  <li>{id}</li>
                ))
              }
            </ul>
          </div>

        </div>

      </div>
    );
  }
}

export default App;