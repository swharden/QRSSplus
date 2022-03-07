import React, { useState, useEffect } from 'react';

function Config(props) {

    const [timestamp, setTimestamp] = useState();

    const setThumbnailCount = props.setThumbnailCount;
    const setIsStichVisible = props.setIsStichVisible;
    const grabberStats = props.grabberStats;

    const handleThumbChange = (event) => {
        const value = event.target.value;
        if (value === "2hr")
            setThumbnailCount(6 * 2);
        else if (value === "8hr")
            setThumbnailCount(6 * 8);
        else
            setThumbnailCount(0);
    }

    const handleStitchChange = (event) => {
        setIsStichVisible(event.target.checked);
    }

    useEffect(() => {

        const updateTimestamp = () => {
            const dt = new Date();
            const ts = leftPad(dt.getUTCHours()) + ":" + leftPad(dt.getUTCMinutes()) + ":" + leftPad(dt.getUTCSeconds());
            setTimestamp(ts);
        }

        updateTimestamp();
        const interval = setInterval(() => { updateTimestamp(); }, 1000);
        return () => clearInterval(interval);
    }, []);


    const leftPad = (num, size = 2, padChar = "0") => {
        num = num.toString();
        while (num.length < size)
            num = padChar + num;
        return num;
    };

    const grabberDataAgeMessage = () => {
        if (!grabberStats)
            return;
        const updatedDate = new Date(grabberStats.created);
        const updatedLinuxTime = updatedDate.getTime() / 1000;
        const nowDate = new Date();
        const nowLinuxTime = nowDate.getTime() / 1000;
        const urlAgeMin = (nowLinuxTime - updatedLinuxTime) / 60;
        return `Last updated on ${updatedDate.toLocaleDateString()} ` +
            `at ${updatedDate.toLocaleTimeString()} ` +
            `(${Math.round(urlAgeMin)} min ago)`;
    }

    return (
        <div className="my-2 border bg-light rounded d-inline-block py-2">

            <div className="d-inline-block mx-2 align-top">
                <div><strong>QRSS Plus</strong></div>
                <div><a href="https://swharden.com/blog/2020-10-03-new-age-of-qrss/#qrss-frequency-bands">
                    QRSS Frequencies</a></div>
                <div><a href="https://github.com/swharden/qrssplus/#request-a-change">Suggest a Grabber</a></div>
                <div><a href="https://github.com/swharden/qrssplus/#request-a-change">Request a Modification</a></div>
            </div>

            <div className="d-inline-block mx-2 align-top">
                <div><strong>Resources</strong></div>
                <div><a href="https://swharden.com/blog/2020-10-03-new-age-of-qrss">Introduction to QRSS</a></div>
                <div><a href="https://groups.io/g/qrssknights">Knights QRSS Mailing List</a></div>
                <div><a href='https://swharden.com/qrss/74/'>74! QRSS Newsletter</a></div>
            </div>

            <div className="d-inline-block mx-2 align-top">
                <div><strong>Settings</strong></div>

                <div className="d-inline-block align-top mx-2">
                    <div>UTC Time:</div>
                    <div><code>{timestamp}</code></div>
                </div>

                <div className="d-inline-block mx-2 align-top">
                    <div>Thumbnails:</div>
                    <select className="form-select" defaultValue={'2hr'} name="thumbnailHistory" onChange={handleThumbChange}>
                        <option value="none">None</option>
                        <option value="2hr">2hr (12)</option>
                        <option value="8hr">8hr (48)</option>
                    </select>
                </div>

                <div className="d-inline-block align-top mx-2">
                    <div>Stitch:</div>
                    <input className="form-check-input" type="checkbox" name="isStichVisible" onChange={handleStitchChange} />&nbsp; 8hr
                </div>
            </div>

            <div className="mx-2 text-muted">
                <code>{grabberDataAgeMessage()}</code>
            </div>

        </div>
    );
}

export default Config;