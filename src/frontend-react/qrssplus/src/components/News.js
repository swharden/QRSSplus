import React, { useState, useEffect } from 'react';
import Dashboard from './Dashboard';

function News(props) {

    const [alertMessage, setAlertMessage] = useState();
    const [alertDate, setAlertDate] = useState();

    useEffect(() => {
        console.log("fetching NOAA alert message...")
        fetch('https://services.swpc.noaa.gov/text/wwv.txt')
            .then(response => response.text())
            .then(text => {
                const lines = text.split("\n");

                const dt = lines[1].replace(":Issued: ", "");
                setAlertDate(dt);

                const goodLines = lines.filter(x => !x.startsWith(":")).filter(x => !x.startsWith("#"));
                const goodMessage = goodLines.join("\n");
                setAlertMessage(goodMessage);
            });
    }, []);

    return (
        <div>

            <section>
                <button
                    className="btn btn-secondary my-2 me-3"
                    type="button"
                    data-bs-toggle="collapse"
                    data-bs-target="#collapseGeophysical"
                    aria-expanded="false"
                    aria-controls="collapseGeophysical">
                    Geophysical Alert
                    </button>

                <button
                    className="btn btn-secondary my-2 me-3"
                    type="button"
                    data-bs-toggle="collapse"
                    data-bs-target="#collapseAurora"
                    aria-expanded="false"
                    aria-controls="collapseAurora">
                    Aurora Forcast
                    </button>

                <button
                    className="btn btn-secondary my-2 me-3"
                    type="button"
                    data-bs-toggle="collapse"
                    data-bs-target="#collapseSunlitMap"
                    aria-expanded="false"
                    aria-controls="collapseSunlitMap">
                    Sunlit Map
                    </button>

                <button
                    className="btn btn-secondary my-2 me-3"
                    type="button"
                    data-bs-toggle="collapse"
                    data-bs-target="#collapseDashboard"
                    aria-expanded="false"
                    aria-controls="collapseDashboard">
                    Grabber Dashboard
                    </button>

                <a
                    className="btn btn-secondary"
                    href="?view=mobile"
                    role="button">
                    Mobile View
                    </a>

            </section>

            <section>
                <div className="collapse" id="collapseGeophysical">
                    <div className="card my-3 d-inline-block">
                        <div className="card-header">
                            <strong>Geophysical Alert Message ({alertDate})</strong>
                        </div>
                        <div className="p-3 mb-0 pb-0">
                            <code><pre>{alertMessage}</pre></code>
                        </div>
                    </div>
                </div>

                <div className="collapse" id="collapseAurora">
                    <div className="card my-3 d-inline-block">
                        <div className="card-header">
                            <strong>NOAA Aurora Forecast</strong>
                        </div>
                        <div className="row p-3">
                            <div className="col-6">
                                <a href="https://services.swpc.noaa.gov/images/aurora-forecast-northern-hemisphere.jpg"
                                    target="_blank" rel="noreferrer" >
                                    <img src="https://services.swpc.noaa.gov/images/aurora-forecast-northern-hemisphere.jpg"
                                        className="img-fluid" alt="northern-hemisphere" />
                                </a>
                            </div>
                            <div className="col-6">
                                <a href="https://services.swpc.noaa.gov/images/aurora-forecast-southern-hemisphere.jpg"
                                    target="_blank" rel="noreferrer" >
                                    <img src="https://services.swpc.noaa.gov/images/aurora-forecast-southern-hemisphere.jpg"
                                        className="img-fluid" alt="southern-hemisphere" />
                                </a>
                            </div>
                        </div>
                    </div>
                </div>

                <div className="collapse" id="collapseSunlitMap">
                    <div className="card my-3 d-inline-block">
                        <div className="card-header">
                            <strong>Sunlit Map with MUF Data</strong>
                        </div>
                        <a href="http://www.hamqsl.com/solar.html">
                            <img src="http://www.hamqsl.com/solarmuf.php" alt="sunlit map"
                                className="img-fluid m-3 border-dark shadow-sm" />
                        </a>
                    </div>
                </div>

                <div className="collapse" id="collapseDashboard">
                    <div className="card my-3 d-inline-block">
                        <div className="card-header">
                            <strong>Grabber Dashboard</strong>
                        </div>
                        <Dashboard grabberStats={props.grabberStats} />
                    </div>
                </div>

            </section>

        </div>
    )
}

export default News;