import React, { useState, useEffect } from 'react';
import GrabberList from './components/GrabberList';
import Config from './components/Config';
import News from './components/News';
import Thumbnails from './components/Thumbnails';
import MobileView from './components/MobileView';
import Banner from './components/Banner';

function App() {

  const [grabberStats, setGrabberStats] = useState();
  const [thumbnailCount, setThumbnailCount] = useState(12);
  const [isStichVisible, setIsStichVisible] = useState(false);

  useEffect(() => {
    // this block runs once at startup
    updateGrabbers();
  }, []);

  const updateGrabbers = () => {
    console.log("UPDATING GRABBERS " + new Date().toISOString());
    const url = 'https://qrssplus.z20.web.core.windows.net/grabbers.json?nocache=' + (new Date()).getTime();
    fetch(url, { 'cache': 'no-store', 'Cache-Control': 'no-cache' })
      .then(response => response.json())
      .then(obj => {
        setGrabberStats(obj);
        console.log(`read ${Object.keys(obj.grabbers).length} grabbers at ${obj.created}`);
      });
  }

  const urlParams = new URLSearchParams(window.location.search);
  const view = urlParams.get('view');
  switch (view) {
    case "mobile":
      return (
        <div className="container-fluid text-center">
          <Banner mobile={true} />
          <MobileView grabberStats={grabberStats} />
        </div>
      )
    default:
      return (
        <div className="container">
          <Banner mobile={false} />
          <Config setThumbnailCount={setThumbnailCount} setIsStichVisible={setIsStichVisible} />
          <News grabberStats={grabberStats} />
          <Thumbnails grabberStats={grabberStats} />
          <GrabberList
            grabberStats={grabberStats}
            thumbnailCount={thumbnailCount}
            isStichVisible={isStichVisible}
          />
        </div>
      );
  }
}

export default App;