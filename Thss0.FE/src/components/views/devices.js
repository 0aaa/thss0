import { Children, useState } from 'react'
import { getRecords } from '../../services/entities'
import { connect } from 'react-redux'

const Devices = props => {
    const [devices, setDevices] = useState();
    getRecords('devices').then(res => {
        setDevices(res.content);
    })
    return <>
        <div id="devicesBody" className="modal-body">
            {(devices
                && <table className="table">
                    <tbody>
                        {Children.toArray(devices.map(device =>
                            <tr>
                                <td>{device.name}</td>
                                <td className="text-center">
                                    {(device.availability && 'ready') || 'busy'}
                                </td>
                                <td className="text-end">
                                    <button id={`${device.name}Btn`}
                                            onClick={async event => { event.preventDefault(); await handleDevice({...props}, device.name, () => event.preventDefault()) }}
                                            disabled={!device.availability} className="btn btn-outline-dark py-0 border-0 border-bottom rounded-0" data-bs-dismiss="modal">
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
            <button className="btn btn-outline-dark col-2 border-0 border-bottom rounded-0" data-bs-dismiss="modal">Close</button>
        </div>
    </>
}

const handleDevice = async (props, name, preventDefault) => {
    console.log(name)
    const data = await getRecords(`devices/${name}`)
    document.getElementById('content').value = data.content
    props.updateContent(props, { target: { id: 'content', value: data.content }, preventDefault: () => preventDefault })
}

const mapStateToProps = state => state

export default connect(mapStateToProps)(Devices)