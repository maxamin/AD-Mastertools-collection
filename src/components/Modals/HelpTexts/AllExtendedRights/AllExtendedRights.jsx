import React from 'react';
import PropTypes from 'prop-types';
import { Tabs, Tab } from 'react-bootstrap';
import General from './General';
import Abuse from './Abuse';
import Opsec from './Opsec';
import References from './References';

const AllExtendedRights = ({
    sourceName,
    sourceType,
    targetName,
    targetType,
    haslaps,
}) => {
    return (
        <Tabs defaultActiveKey={1} id='help-tab-container' justified>
            <Tab eventKey={1} title='Info'>
                <General
                    sourceName={sourceName}
                    sourceType={sourceType}
                    targetName={targetName}
                    targetType={targetType}
                />
            </Tab>
            <Tab eventKey={2} title='Abuse Info'>
                <Abuse
                    sourceName={sourceName}
                    sourceType={sourceType}
                    targetName={targetName}
                    targetType={targetType}
                    haslaps={haslaps}
                />
            </Tab>
            <Tab eventKey={3} title='Opsec Considerations'>
                <Opsec />
            </Tab>
            <Tab eventKey={4} title='References'>
                <References />
            </Tab>
        </Tabs>
    );
};

AllExtendedRights.propTypes = {
    sourceName: PropTypes.string,
    sourceType: PropTypes.string,
    targetName: PropTypes.string,
    targetType: PropTypes.string,
};
export default AllExtendedRights;
