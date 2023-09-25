import React, { Children } from 'react'
import { connect } from 'react-redux'
import { NavLink, useNavigate, useParams } from 'react-router-dom'
import { updateContent } from "../../actionCreator/actionCreator"
import { getRecords } from '../../services/entities'
import { AUTH_TOKEN } from '../../config/consts'
import { UseRedirect, UseUpdate } from '../../config/hooks'
import { HOME_PATH } from '../../config/consts'
import { Modal } from 'bootstrap'

const List = props => {
    const params = useParams()
    const navigate = useNavigate()
    const isAuthenticated = sessionStorage.getItem(AUTH_TOKEN)
    let path = params.entityName
    if (params.toFind) {
        path = `${params.entityName}/${params.toFind}`
        if (params.entityName !== 'users') {
            path = `search/${path}`
        }
    }
    if (!isAuthenticated && ['users/client', 'procedures', 'results'].includes(path)) {
        UseRedirect(HOME_PATH)
        const modal = document.getElementById('loginModal')
        modal && new Modal(modal).show()
    }
    UseUpdate(props, path)
    let pagCoef = 0
    switch (props.currentPage) {
        case 1:
            pagCoef = 1
            break
        case 2:
            pagCoef = 0
            break
        case props.totalPages:
        default:
            pagCoef = -1
    }
    const pagination = []
    pagination.push(props.currentPage - 1 + pagCoef)
    for (let index = -1; index < 2 && index < props.totalPages - 1; index++) {
        pagination.push(props.currentPage + index + pagCoef)
    }
    pagination.push(props.currentPage + 1 + pagCoef)
    return (
        <div className="vh-100">
            <h4 className="d-flex">{params.entityName.replace(/^./, params.entityName[0].toUpperCase())}
                <div className="btn-group w-75 d-flex flex-wrap ms-auto me-2">
                    {isAuthenticated && params.entityName !== 'substances'
                        && <NavLink to={`/add/${params.entityName}`} className="btn btn-outline-dark border-0 border-bottom rounded-0">Add new</NavLink>
                    }
                    <button onClick={() => navigate(-1)} className="btn btn-outline-dark border-0 border-bottom rounded-0">Back</button>
                    {props.content
                        && <>
                            <a href={`data:application/octet-stream,${encodeURIComponent(JSON.stringify(props.content))}`}
                                    download={`${Date.now() + params.entityName}.txt`}
                                    className="btn btn-outline-dark border-0 border-bottom">
                                Download
                            </a>
                            <select id="order" onChange={event =>
                                    props.updateContent({...props, order: event.target.value}, path, event)}
                                    defaultValue="Order"
                                    className="btn btn-outline-dark border-0 border-bottom">
                                <option disabled hidden>Order</option>
                                {Children.toArray([...Object.entries({true : 'ascendent', false : 'descendent'})].map(e =>
                                    <option value={e[0]}>{e[1]}</option>
                                ))}
                            </select>
                            <select id="print-by" onChange={event =>
                                    props.updateContent({...props, printBy: +event.target.value}, path, event)}
                                    defaultValue="Print by"
                                    className="btn btn-outline-dark border-0 border-bottom rounded-0">
                                <option disabled hidden>Print by</option>
                                {Children.toArray([...Array(3).keys()].map(i =>
                                    <option value={(i + 1) * 20}>{(i + 1) * 20}</option>
                                ))}
                            </select>
                        </>
                    }
                </div>
            </h4>
            {(props.content
                && <div className="d-flex flex-column">
                    <div id="list-error" className="alert alert-danger d-none"></div>
                    <table className="table">
                        <thead>
                            <tr>
                                <th className="p-0">
                                    <button name="name-order"
                                            onClick={event =>
                                                props.updateContent({...props, localOrder: !props.localOrder}, path, event)}
                                            className="btn btn-outline-dark border-0 rounded-0 w-100 text-start p-3">
                                        Name {(props.localOrder && <>&darr;</>) || <>&uarr;</>}
                                    </button>
                                </th>
                                {isAuthenticated && params.entityName !== 'substances'
                                    && <>
                                        <th />
                                        <th />
                                    </>
                                }
                            </tr>
                        </thead>
                        <tbody>
                            {Children.toArray(props.content.map(cntnt =>
                                <tr id={cntnt.id}>
                                    <td className="p-0">
                                        <NavLink to={`/details/${params.entityName}/${cntnt.id}`} className="btn btn-outline-dark border-0 rounded-0 w-100 text-start p-3">
                                            {cntnt.name ?? cntnt.userName ?? cntnt.obtainmentTime}
                                        </NavLink>
                                    </td>
                                    {isAuthenticated && params.entityName !== 'substances'
                                        && <>
                                            <td className="p-0">
                                                <NavLink to={`/edit/${params.entityName}/${cntnt.id}`} className="btn btn-outline-dark border-0 rounded-0 w-100 text-start p-3">
                                                    Edit
                                                </NavLink>
                                            </td>
                                            <td className="p-0">
                                                <NavLink to={`/delete/${params.entityName}/${cntnt.id}`} className="btn btn-outline-dark border-0 rounded-0 w-100 text-start p-3 ms-4">
                                                    Delete
                                                </NavLink>
                                            </td>
                                        </>
                                    }
                                </tr>
                            ))}
                        </tbody>
                    </table>
                    {props.totalPages > 1
                        && <ul className="pagination mx-auto">
                            {Children.toArray(pagination
                                .map((pageNum, index) => {
                                    switch (index) {
                                        case 0:
                                            return props.currentPage !== 1
                                                && <li className="page-item">
                                                    <button onClick={event =>
                                                                props.updateContent({...props, currentPage: pageNum - pagCoef}, path, event)}
                                                            className="page-link bg-dark text-white rounded-0">
                                                        Back
                                                    </button>
                                                </li>
                                        case pagination.length - 1:
                                            return props.currentPage !== props.totalPages
                                                && <li className="page-item">
                                                    <button onClick={event =>
                                                                props.updateContent({...props, currentPage: pageNum - pagCoef}, path, event)}
                                                            className="page-link bg-dark text-white rounded-0">
                                                        Forward
                                                    </button>
                                                </li>
                                        default:
                                            return <li className="page-item">
                                                    <button onClick={event =>
                                                                props.updateContent({...props, currentPage: pageNum}, path, event)}
                                                            className="page-link bg-dark text-white rounded-0">
                                                        {pageNum.toString()}
                                                    </button>
                                                </li>
                                    }
                                }))
                            }
                        </ul>
                    }
                </div>)
                || <div className="h-75 d-flex justify-content-center align-items-center gap-1">
                    {Children.toArray([...Array(3).keys()].map(() =>
                        <div className="spinner-grow text-primary" role="status">
                            <span className="visually-hidden">Loading...</span>
                        </div>
                    ))}
                </div>
            }
        </div>
    )
}

const mapStateToProps = state => state

const mapDispatchToProps = dispatch => {
    return {
        updateContent: async (stateCopy, path, event = null) => {
            event?.preventDefault()
            if (event?.target.name !== 'name-order') {
                if (stateCopy.currentPage <= 0) {
                    stateCopy.currentPage = 1
                } else if (stateCopy.currentPage > stateCopy.totalPages) {
                    stateCopy.currentPage = stateCopy.totalPages
                }
                const data = await getRecords(path, stateCopy.order, stateCopy.printBy, stateCopy.currentPage)
                if (!data) {
                    return
                }
                stateCopy.content = data.content
                stateCopy.totalPages = Math.ceil(data.total_amount / stateCopy.printBy)
            }
            dispatch(updateContent(stateCopy.content, stateCopy.totalPages, stateCopy.localOrder, stateCopy.currentPage))
        }
    }
}
export default connect(mapStateToProps, mapDispatchToProps)(List)