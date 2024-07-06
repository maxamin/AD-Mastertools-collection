import React from 'react';
import PropTypes from 'prop-types';
import { groupSpecialFormat } from '../Formatter';

const General = ({ sourceName, sourceType, targetName }) => {
    return (
        <>
            <p>
                {groupSpecialFormat(sourceType, sourceName)} is added to the
                msds-AllowedToActOnBehalfOfOtherIdentity attribute on the
                computer {targetName}.
            </p>

            <p>
                An attacker can use this account to execute a modified
                S4U2self/S4U2proxy abuse chain to impersonate any domain user to
                the target computer system and receive a valid service ticket
                "as" this user.
            </p>

            <p>
                One caveat is that impersonated users can not be in the
                "Protected Users" security group or otherwise have delegation
                privileges revoked. Another caveat is that the principal added
                to the msDS-AllowedToActOnBehalfOfOtherIdentity DACL *must* have
                a service principal name (SPN) set in order to successfully
                abuse the S4U2self/S4U2proxy process. If an attacker does not
                currently control an account with a SPN set, an attacker can
                abuse the default domain MachineAccountQuota settings to add a
                computer account that the attacker controls via the Powermad
                project.
            </p>
        </>
    );
};

General.propTypes = {
    sourceName: PropTypes.string,
    sourceType: PropTypes.string,
    targetName: PropTypes.string,
};

export default General;
