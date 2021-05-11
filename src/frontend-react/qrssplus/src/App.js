import React from 'react';
import GrabberThumbnail from './GrabberThumbnail'
import GrabberDetails from './GrabberDetails'

class App extends React.Component {

  constructor(props) {
    super(props);
    this.state = {
      grabbersJson: {},
      grabbers: {},
      timeNow: new Date(),
      lastUpdate: null,
      nextUpdate: null
    };
  }

  componentDidMount() {
    this.timerID = setInterval(() => this.tick(), 1000);
  }

  tick() {
    const dt = new Date();

    if (dt > this.state.nextUpdate) {
      this.onUpdateGrabbers();
    }

    const roundedMinutes = Math.floor(dt.getMinutes() / 10) * 10;
    var nextUpdate = new Date(dt.getFullYear(), dt.getMonth(), dt.getDate(), dt.getHours(), roundedMinutes, 0);
    nextUpdate = new Date(nextUpdate.getTime() + 5 * 60 * 1000);
    if (nextUpdate < dt)
      nextUpdate = new Date(nextUpdate.getTime() + 10 * 60 * 1000);
    this.setState({ timeNow: dt, nextUpdate: nextUpdate });
  }

  onUpdateGrabbers(maxCount = 999) {
    console.log("UPDATING " + new Date().toISOString());
    fetch('https://qrssplus.z20.web.core.windows.net/grabbers.json')
      .then(response => response.json())
      .then(obj => {
        this.setState({ grabbersJson: obj });
        this.setState({ grabbers: Object.keys(obj.grabbers).slice(0, maxCount).map(x => (obj.grabbers[x])) });
        this.setState({ lastUpdate: new Date() });
        console.log(`read ${Object.keys(obj.grabbers).length} grabbers`);
      });
  }

  renderThumbnails() {
    return (
      <div className="">
        {
          Object.keys(this.state.grabbers)
            .filter(id => this.state.grabbers[id].urls.length > 0)
            .map(id => (
              <GrabberThumbnail key={id} grabber={this.state.grabbers[id]} />
            ))
        }
      </div>
    )
  }

  renderDetails() {
    return (
      <section>
        {
          Object.keys(this.state.grabbers)
            .filter(id => this.state.grabbers[id].urls.length > 0)
            .map(id => (
              <GrabberDetails key={id} grabber={this.state.grabbers[id]} />
            ))
        }
      </section>
    )
  }

  renderDashboard() {
    if (!this.state.lastUpdate)
      return (<h1>Loading...</h1>)

    return (
      <div className="my-5">
        <h1>Dashboard</h1>
        <div>Last Update: <code>{this.getTimestamp(this.state.lastUpdate)} UTC</code></div>
        <table className="table table-hover border shadow">
          <thead className="bg-dark text-light">
            <tr>
              <th>ID</th>
              <th>Name</th>
              <th>Callsign</th>
              <th>Location</th>
              <th>Image URL</th>
              <th>Website URL</th>
              <th>Unique Grabs</th>
              <th>Last Update</th>
            </tr>
          </thead>
          <tbody>
            {
              Object.keys(this.state.grabbers)
                .map(id => (this.state.grabbers[id]))
                .map(grabber => (
                  <tr key={grabber.id} className={grabber.lastUniqueAgeDays > 7 ? "table-danger" : ""}>
                    <td><code>{grabber.id}</code></td>
                    <td>{grabber.name}</td>
                    <td>{grabber.callsign}</td>
                    <td>{grabber.location}</td>
                    <td><a href={grabber.imageUrl}>image</a></td>
                    <td><a href={grabber.siteUrl}>site</a></td>
                    <td>{grabber.urls.length > 0 ? grabber.urls.length : "--"}</td>
                    <td>{grabber.lastUniqueAgeDays >= 1 ? grabber.lastUniqueAgeDays + " days" : ""}</td>
                  </tr>
                ))
            }
          </tbody>
        </table>
      </div>
    )
  }

  leftPad(num, size = 2, padChar = "0") {
    num = num.toString();
    while (num.length < size)
      num = padChar + num;
    return num;
  }

  getTimestamp(dt) {
    if (!dt)
      return "updating...";
    return this.leftPad(dt.getUTCHours()) + ":"
      + this.leftPad(dt.getUTCMinutes()) + ":"
      + this.leftPad(dt.getUTCSeconds()) + " UTC";
  }

  renderTimer() {
    return (
      <div className="d-inline-block bg-light border rounded p-2 m-3">
        <div>Current Time: <code>{this.getTimestamp(this.state.timeNow)}</code></div>
        <div>Last Update: <code>{this.getTimestamp(this.state.lastUpdate)}</code></div>
        <div>Next Update: <code>{this.getTimestamp(this.state.nextUpdate)}</code></div>
      </div>
    )
  }

  render() {
    return (
      <div>
        {this.renderTimer()}
        {this.renderThumbnails()}
        {this.renderDetails()}
        {this.renderDashboard()}
      </div>
    );
  }
}

export default App;