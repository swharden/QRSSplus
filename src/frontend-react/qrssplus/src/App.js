import React from 'react';

class App extends React.Component {

  constructor(props) {
    super(props);
    this.state = {
      grabbersJson: {},
      grabbers: {},
      timeNow: new Date(),
      lastUpdate: null,
      nextUpdate: null,
      isAutoUpdateEnabled: false,
      thumbnailHistory: "2hr",
      isStichVisible: false,
      wwvAlertMessage: "Loading...",
      wwvAlertDate: "Loading..."
    };
    this.handleInputChange = this.handleInputChange.bind(this);
  }

  componentDidMount() {
    this.timerID = setInterval(() => this.tick(), 1000);
    this.onUpdateMessages();
    this.onUpdateGrabbers();
  }

  tick() {
    const dt = new Date();

    if (!this.state.isAutoUpdateEnabled) {
      this.setState({ timeNow: dt });
      return;
    }

    if (dt >= this.state.nextUpdate) {
      this.onUpdateGrabbers();
    }

    const roundedMinutes = Math.floor(dt.getMinutes() / 10) * 10;
    var nextUpdate = new Date(dt.getFullYear(), dt.getMonth(), dt.getDate(), dt.getHours(), roundedMinutes, 0);
    nextUpdate = new Date(nextUpdate.getTime() + 5 * 60 * 1000);
    if (nextUpdate < dt)
      nextUpdate = new Date(nextUpdate.getTime() + 10 * 60 * 1000);
    this.setState({ timeNow: dt, nextUpdate: nextUpdate });
  }

  onUpdateMessages() {
    console.log("UPDATING MESSAGES " + new Date().toISOString());
    fetch('https://services.swpc.noaa.gov/text/wwv.txt')
      .then(response => response.text())
      .then(text => {
        const lines = text.split("\n");
        const dt = lines[1].replace(":Issued: ", "");
        const goodLines = lines.filter(x => !x.startsWith(":")).filter(x => !x.startsWith("#"));
        const goodMessage = goodLines.join("\n");
        this.setState({ wwvAlertMessage: goodMessage, wwvAlertDate: dt });
      });
  }

  onUpdateGrabbers() {
    console.log("UPDATING GRABBERS " + new Date().toISOString());
    fetch(
      'https://qrssplus.z20.web.core.windows.net/grabbers.json?nocache=' + (new Date()).getTime(),
      { 'cache': 'no-store', 'Cache-Control': 'no-cache' }
    )
      .then(response => response.json())
      .then(obj => {
        this.setState({ grabbersJson: obj, grabbers: obj.grabbers, lastUpdate: new Date() });
        console.log(`read ${Object.keys(obj.grabbers).length} grabbers at ${obj.created}`);
      });
  }

  renderActivityIcon(ageMinutes) {
    if (ageMinutes < 35)
      return (<span className="badge bg-success">Active</span>)
    if (ageMinutes < (60 * 24))
      return (<span className="badge bg-warning">Inactive ({ageMinutes} minutes)</span>)
    return (<span className="badge bg-danger">Offline ({ageMinutes / 60 / 24} days)</span>)
  }

  basename(url) {
    return url.substr(url.lastIndexOf('/') + 1);
  }

  timestampFromUrl(url) {
    const parts = this.basename(url).split(" ")[1].split('.');
    const timestamp = parts[3] + ":" + parts[4];
    return timestamp;
  }

  widthFromUrl(url) {
    return this.basename(url).split(" ")[2].split('x')[0];
  }

  heightFromUrl(url) {
    return this.basename(url).split(" ")[2].split('x')[1];
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

  handleInputChange(event) {
    const target = event.target;
    const value = target.type === 'checkbox' ? target.checked : target.value;
    const name = target.name;

    this.setState({
      [name]: value
    });
  }

  /**
   * section at the top of the page
   */
  renderTimer() {

    const grabbers = Object.keys(this.state.grabbers).map(x => this.state.grabbers[x]);
    const grabberTotalCount = grabbers.length;
    const grabberActiveCount = grabbers.filter(x => x.urls.length > 0).length;
    const grabberActivePercentage = grabberActiveCount / grabberTotalCount * 100;

    return (
      <div className="">

        <div className="d-inline-block bg-light border rounded p-2 m-2 align-top">
          <div>Current Time: <code>{this.getTimestamp(this.state.timeNow)}</code></div>
          <div>Last Update: <code>{this.getTimestamp(this.state.lastUpdate)}</code></div>
          <div>Next Update: <code>{this.state.isAutoUpdateEnabled ? this.getTimestamp(this.state.nextUpdate) : "disabled"}</code></div>
          <div className="form-check mt-2">
            <input
              className="form-check-input"
              type="checkbox"
              name="isAutoUpdateEnabled"
              id="isAutoUpdateEnabled"
              checked={this.state.isAutoUpdateEnabled}
              onChange={this.handleInputChange}
            />
            Auto-Update
          </div>
        </div>

        <div className="d-inline-block bg-light border rounded p-2 m-2 align-top">

          <div className="">
            Thumbnails:
        </div>

          <select className="form-select" defaultValue={'2hr'} name="thumbnailHistory" onChange={this.handleInputChange}>
            <option value="none">None</option>
            <option value="2hr">2hr (12)</option>
            <option value="8hr">8hr (48)</option>
          </select>

          <div className="form-check mt-3">
            <input className="form-check-input" type="checkbox" name="isStichVisible" onChange={this.handleInputChange} />
            8hr stitch
          </div>
        </div>

        <div className="d-inline-block p-2 m-2 align-top">
          <div className="mb-2">
            QRSS Plus:
        </div>
          <ul>
            <li><a href="https://swharden.com/blog/2020-10-03-new-age-of-qrss/#qrss-frequency-bands">QRSS Frequency
              Bands</a></li>
            <li><a href="https://github.com/swharden/qrssplus/#request-a-change">Suggest a Grabber</a></li>
            <li><a href="https://github.com/swharden/qrssplus/#request-a-change">Request a Modification</a></li>
          </ul>
        </div>

        <div className="d-inline-block p-2 m-2 align-top">
          <div className="mb-2">
            Additional Resources:
        </div>
          <ul>
            <li><a href="https://groups.io/g/qrssknights">Knights QRSS Mailing List</a></li>
            <li><a href='https://swharden.com/qrss/74/'>74!, The Knights QRSS Newsletter</a></li>

            <li><a href="https://swharden.com/blog/2020-10-03-new-age-of-qrss">The
              New Age of QRSS</a></li>
          </ul>
        </div>

        <div className="mx-2">
          <code>
            {
              grabberActivePercentage
                ? `${grabberActiveCount} of ${grabberTotalCount} grabs are active (${Math.round(grabberActivePercentage)}%)`
                : `loading...`
            }
          </code>
        </div>

      </div>
    )
  }

  /**
   * section at the top of the page
   */
  renderMainThumbnails() {
    return (
      <div className="">
        {
          Object.keys(this.state.grabbers)
            .filter(id => this.state.grabbers[id].urls.length > 0)
            .map(id => this.renderMainThumbnail(this.state.grabbers[id]))
        }
      </div>
    )
  }

  /**
   * thumbnail at the top of the page that shows a grabber's name
   */
  renderMainThumbnail(grabber) {
    return (
      <div className="d-inline-block m-2" key={grabber.id}>
        <div>{grabber.id}</div>
        <div>
          <a href={"#" + grabber.id}>
            <img
              src={grabber.urls[grabber.urls.length - 1]}
              alt={grabber.id}
              width="150"
              height="100"
              className="shadow border" />
          </a>
        </div>
      </div>
    );
  }

  /**
   * thumbnail below a grabber that just shows the time
   */
  renderDatedThumbnail(url) {
    return (
      <div className="d-inline-block m-2" key={this.basename(url)}>
        <div className="text-muted">{this.timestampFromUrl(url)}</div>
        <div>
          <a href={url}>
            <img
              className="border border-dark shadow"
              alt="alt"
              src={url + "-thumb-auto.jpg"}
              width="150"
              height="100" />
          </a>
        </div>
      </div>
    )
  }

  renderDetailsForGrabber(grabber, maxThumbnailCount, showStitch) {
    const latestUrl = grabber.urls[grabber.urls.length - 1];
    return (
      <div id={grabber.id} key={latestUrl}>
        <h2 className="my-5 mb-0 display-4 my-0 fw-normal">{grabber.id}</h2>

        <div className="fs-5 my-0">
          {grabber.name} (<a href={"https://www.qrz.com/lookup/" + grabber.callsign}>{grabber.callsign}</a>)
                in {grabber.location} (<a href={grabber.siteUrl}>website</a>)
                &nbsp;
                {this.renderActivityIcon(grabber.lastUniqueAgeMinutes)}
        </div>

        <div>
          <div className="text-muted">{this.timestampFromUrl(latestUrl)}</div>
          <div className="mt-1 mb-1">
            <a href={latestUrl}>
              <img
                className="border border-dark shadow figure-img img-fluid"
                src={latestUrl}
                alt={grabber.id}
                width={this.widthFromUrl(latestUrl)}
                height={this.heightFromUrl(latestUrl)}
              />
            </a>
          </div>
        </div>

        {
          Object.keys(grabber.urls)
            .map(x => grabber.urls[x])
            .reverse()
            .slice(0, maxThumbnailCount)
            .map(x => this.renderDatedThumbnail(x))
        }

        {
          showStitch ? (
            <div className="text-nowrap overflow-scroll m-2 bg-light border shadow" >
              {
                Object.keys(grabber.urls)
                  .map(x => grabber.urls[x])
                  .map(url => (
                    <a href={url} key={url}>
                      <img src={url + "-thumb-skinny.jpg"} alt={url} width="50" height="500" />
                    </a>
                  ))
              }
            </div>
          ) : ""
        }

      </div>
    );
  }

  getThumbnailCount() {
    if (this.state.thumbnailHistory === "8hr")
      return 6 * 8;
    if (this.state.thumbnailHistory === "2hr")
      return 6 * 2;
    return 0;
  }

  renderDetailsForAllGrabbers() {
    return (
      <section>
        {
          Object.keys(this.state.grabbers)
            .filter(id => this.state.grabbers[id].urls.length > 0)
            .map(id => this.renderDetailsForGrabber(this.state.grabbers[id], this.getThumbnailCount(), this.state.isStichVisible))
        }
      </section>
    )
  }

  grabberRowClass(grabber) {
    if (grabber.urls.length > 0)
      return ""
    if (grabber.lastUniqueAgeDays >= 7)
      return "table-danger"
    else return "table-warning"
  }

  renderDashboard() {
    if (!this.state.lastUpdate)
      return (<code>Loading...</code>)

    return (
      <table className="table table-hover">
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
                <tr key={grabber.id} className={this.grabberRowClass(grabber)}>
                  <td><code>{grabber.id}</code></td>
                  <td>{grabber.name}</td>
                  <td>{grabber.callsign}</td>
                  <td>{grabber.location}</td>
                  <td><a href={grabber.imageUrl} target="_blank" rel="noreferrer">image</a></td>
                  <td><a href={grabber.siteUrl} target="_blank" rel="noreferrer">site</a></td>
                  <td>{grabber.urls.length > 0 ? grabber.urls.length : "--"}</td>
                  <td>{grabber.lastUniqueAgeDays >= 1 ? grabber.lastUniqueAgeDays + " days" : ""}</td>
                </tr>
              ))
          }
        </tbody>
      </table>
    )
  }

  renderAccordion() {
    return (
      <section>
        <div className="accordion my-5 shadow" id="accordionExample">

          <div className="accordion-item">
            <h2 className="accordion-header" id="headingConfig">
              <button className="accordion-button" type="button" data-bs-toggle="collapse"
                data-bs-target="#collapseConfig" aria-expanded="true" aria-controls="collapseConfig">
                Configuration
            </button>
            </h2>
            <div id="collapseConfig" className="accordion-collapse collapse show" aria-labelledby="headingConfig"
              data-bs-parent="#accordionExample">
              <div className="accordion-body">
                {this.renderTimer()}
              </div>
            </div>
          </div>

          <div className="accordion-item">
            <h2 className="accordion-header" id="headingWwvMessage">
              <button className="accordion-button collapsed" type="button" data-bs-toggle="collapse"
                data-bs-target="#collapseWwvMessage" aria-expanded="false" aria-controls="collapseWwvMessage">
                WWV Geophysical Alert ({this.state.wwvAlertDate})
            </button>
            </h2>
            <div id="collapseWwvMessage" className="accordion-collapse collapse" aria-labelledby="headingWwvMessage"
              data-bs-parent="#accordionExample">
              <div className="accordion-body">
                <pre><code>{this.state.wwvAlertMessage}</code></pre>
              </div>
            </div>
          </div>

          <div className="accordion-item">
            <h2 className="accordion-header" id="headingForecastImages">
              <button className="accordion-button collapsed" type="button" data-bs-toggle="collapse"
                data-bs-target="#collapseForecastImages" aria-expanded="false" aria-controls="collapseForecastImages">
                NOAA Auroral Forecasts
            </button>
            </h2>
            <div id="collapseForecastImages" className="accordion-collapse collapse" aria-labelledby="headingForecastImages"
              data-bs-parent="#accordionExample">
              <div className="accordion-body">
                <div className="container">
                  <div className="row">
                    <div className="col-6">
                      <img src="https://services.swpc.noaa.gov/images/aurora-forecast-northern-hemisphere.jpg"
                        className="img-fluid" alt="northern-hemisphere" />
                    </div>
                    <div className="col-6">
                      <img src="https://services.swpc.noaa.gov/images/aurora-forecast-southern-hemisphere.jpg"
                        className="img-fluid" alt="southern-hemisphere" />
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div className="accordion-item">
            <h2 className="accordion-header" id="headingDashboard">
              <button className="accordion-button collapsed" type="button" data-bs-toggle="collapse"
                data-bs-target="#collapseDashboard" aria-expanded="false" aria-controls="collapseDashboard">
                Grabber Dashboard
            </button>
            </h2>
            <div id="collapseDashboard" className="accordion-collapse collapse" aria-labelledby="headingDashboard"
              data-bs-parent="#accordionExample">
              <div className="accordion-body">
                {this.renderDashboard()}
              </div>
            </div>
          </div>

        </div>
      </section>)
  }

  render() {
    return (
      <div>
        {this.renderAccordion()}
        {this.renderMainThumbnails()}
        {this.renderDetailsForAllGrabbers()}
      </div>
    );
  }
}

export default App;