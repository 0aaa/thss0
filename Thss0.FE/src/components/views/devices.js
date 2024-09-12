import { Children, useState } from 'react'
import { getRecord, getRecords } from '../../services/entities'
import { connect } from 'react-redux'

const Devices = props => {
    const [devices, setDevices] = useState();
    if (!devices) {
        getRecords('device', props.printBy, props.currentPage, props.globalOrder).then(res => {
            setDevices(res?.content);
        })
    }
    return <>
        <div id="devicesBody" className="modal-body">
            {(devices
                && <table className="table">
                    <tbody>
                        {Children.toArray(devices.map(d =>
                            <tr>
                                <td>{d.name}</td>
                                <td className="text-center">
                                    {(d.availability && 'ready') || 'busy'}
                                </td>
                                <td className="text-end">
                                    <button id={`${d.name}Btn`}
                                            onClick={async e => { e.preventDefault(); await handleDevice({ ...props }, d.name, () => e.preventDefault()) }}
                                            disabled={!d.availability} className={`btn btn-outline-${props.btnColor} py-0 border-0 border-bottom rounded-0`} data-bs-dismiss="modal">
                                        Read
                                    </button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>)
                || <div className="h-75 d-flex justify-content-center align-items-center gap-1">
                    {Children.toArray([...Array(3).keys()].map(() =>
                        <div className="spinner-grow text-primary" role="status">
                            <span className="visually-hidden">Loading...</span>
                        </div>))}
                </div>
            }
        </div>
        <div className="modal-footer">
            <button className={`btn btn-outline-${props.btnColor} col-2 border-0 border-bottom rounded-0`} data-bs-dismiss="modal">Close</button>
        </div>
    </>
}

const handleDevice = async (props, name, preventDefault) => {
    //const data = await getRecords(`device/${name}`, props.printBy, props.currentPage, props.globalOrder)
    const data = await getRecord(`device/${name}`)
    console.log(data)
    document.getElementById('content-input').value = data.content
    //updateContent(props, { target: { id: 'content', value: data.content }, preventDefault: () => preventDefault })
}

const mapStateToProps = state => state

export default connect(mapStateToProps)(Devices)