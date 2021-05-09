import React from 'react';
import GrabberThumbnail from './GrabberThumbnail'
import GrabberDetails from './GrabberDetails'

class App extends React.Component {

  constructor(props) {
    super(props);
    this.state = {
      grabbersJson: {},
      grabbers: {}
    };
    this.onUpdateGrabbers();
  }

  onUpdateGrabbers(maxCount = 999) {
    this.setState({ grabbersJson: "downloading..." });
    fetch('https://qrssplus.z20.web.core.windows.net/grabbers.json')
      .then(response => response.json())
      .then(obj => {
        this.setState({ grabbersJson: obj });
        this.setState({ grabbers: Object.keys(obj.grabbers).slice(0, maxCount).map(x => (obj.grabbers[x])) });
      });
  }

  renderSummary() {
    return (
      <div>
        Displaying {Object.keys(this.state.grabbers).length} active grabbers
      </div>
    )
  }

  renderThumbnails() {
    return (
      <div className="my-5">
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
    return (
      <div className="m-5 p-3">
        <h1>Dashboard</h1>
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

  render() {
    return (
      <div className="container">
        <h1>QRSS Plus</h1>
        <h3>Automatically Updating Active QRSS Grabbers List</h3>

        {this.renderThumbnails()}
        {this.renderDetails()}
        {this.renderDashboard()}


      </div>
    );
  }
}

export default App;