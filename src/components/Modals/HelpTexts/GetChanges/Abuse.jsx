import React from 'react';

const Abuse = () => {
    return (
        <>
            <p>
                With both GetChanges and GetChangesAll privileges in BloodHound,
                you may perform a dcsync attack to get the password hash of an
                arbitrary principal using mimikatz:
            </p>
            <pre>
                <code>
                    {
                        'lsadump::dcsync /domain:testlab.local /user:Administrator'
                    }
                </code>
            </pre>
            <p>
                You can also perform the more complicated ExtraSids attack to
                hop domain trusts. For information on this see the blog post by
                harmj0y in the references tab.
            </p>
        </>
    );
};

export default Abuse;
