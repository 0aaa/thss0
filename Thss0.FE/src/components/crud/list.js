import React, { Children } from 'react'
import { connect } from 'react-redux'
import { useNavigate, useParams } from 'react-router-dom'
import { updateContent, updateDetailed } from "../../actionCreator/actionCreator"
import { getRecords } from '../../services/entities'
import { AUTH_TOKEN } from '../../config/consts'
import { UseRedirect, UseUpdate } from '../../config/hooks'
import { HOME_PATH } from '../../config/consts'
import { Modal } from 'bootstrap'
import Details from './details'
import AddRouter from './add'
import EditRouter from './edit'
import Delete from './delete'

const List = props => {
    const params = useParams()
    const navigate = useNavigate()
    const isAuthenticated = sessionStorage.getItem(AUTH_TOKEN)
    const path = (params.toFind && `search/${params.entityName}/${params.toFind}`) || params.entityName
    if (!isAuthenticated && ['client', 'procedures', 'results'].includes(path)) {
        UseRedirect(HOME_PATH)
    }
    UseUpdate(props, path)
    const pagCoef = getPagCoef(props)
    const pagination = getPagination(props, pagCoef)
    return <div className="vh-100">
            <h4 className="d-flex">{params.entityName.replace(/^./, params.entityName[0].toUpperCase())}
                <div className="btn-group w-75 d-flex flex-wrap ms-auto me-2">
                    {isAuthenticated && params.entityName !== 'substances'
                        && <button className={`btn btn-outline-${props.btnColor} border-0 border-bottom rounded-0`} data-bs-toggle="offcanvas" data-bs-target="#offcanvasAdd" aria-controls="offcanvasAdd">Add new</button>
                    }
                    <button onClick={() => navigate(-1)} className={`btn btn-outline-${props.btnColor} border-0 border-bottom rounded-0`}>Back</button>
                    {props.content
                        && <>
                            <a href={`data:application/octet-stream,${encodeURIComponent(JSON.stringify(props.content))}`}
                                    download={`${Date.now() + params.entityName}.txt`}
                                    className={`btn btn-outline-${props.btnColor} border-0 border-bottom`}>
                                Download
                            </a>
                            <select id="order" onChange={event =>
                                    props.updateContent({...props, globalOrder: event.target.value}, path, event)}
                                    defaultValue="Order"
                                    className={`btn btn-outline-${props.btnColor} border-0 border-bottom`}>
                                <option disabled hidden>Order</option>
                                {Children.toArray(['ascendent', 'descendent'].map(globalOrder =>
                                    <option value={globalOrder === 'ascendent'}>{globalOrder}</option>
                                ))}
                            </select>
                            <select id="print-by" onChange={event =>
                                    props.updateContent({...props, printBy: +event.target.value}, path, event)}
                                    defaultValue="Print by"
                                    className={`btn btn-outline-${props.btnColor} border-0 border-bottom rounded-0`}>
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
                                                props.updateContent({...props, inPageOrder: !props.inPageOrder}, path, event)}
                                            className={`btn btn-outline-${props.btnColor} border-0 rounded-0 w-100 text-start p-3`}>
                                        Name {(props.inPageOrder && <>&darr;</>) || <>&uarr;</>}
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
                            {Children.toArray(props.content.map(entityItem =>
                                <tr id={entityItem.id}>
                                    <td className="p-0">
                                        <button onClick={event => props.updateDetailed(event, `${path}/${entityItem.id}`)} className={`btn btn-outline-${props.btnColor} border-0 rounded-0 w-100 text-start p-3`} data-bs-toggle="offcanvas" data-bs-target="#offcanvasDetails" aria-controls="offcanvasDetails">
                                            {entityItem.name ?? entityItem.userName ?? entityItem.obtainmentTime}
                                        </button>
                                    </td>
                                    {isAuthenticated && params.entityName !== 'substances'
                                        && <>
                                            <td className="p-0">
                                                <button onClick={event => props.updateDetailed(event, `${path}/${entityItem.id}`)} className={`btn btn-outline-${props.btnColor} border-0 rounded-0 w-100 text-start p-3`} data-bs-toggle="offcanvas" data-bs-target="#offcanvasEdit" aria-controls="offcanvasEdit">
                                                    Edit
                                                </button>
                                            </td>
                                            <td className="p-0">
                                                <button onClick={event => props.updateDetailed(event, `${path}/${entityItem.id}`)} className={`btn btn-outline-${props.btnColor} border-0 rounded-0 w-100 text-start p-3`} data-bs-toggle="offcanvas" data-bs-target="#offcanvasDelete" aria-controls="offcanvasDelete">
                                                    Delete
                                                </button>
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
            <Details />
            <AddRouter entityName={params.entityName} />
            <EditRouter entityName={params.entityName} />
            <Delete entityName={params.entityName} />
        </div>
}

const mapStateToProps = state => state

const mapDispatchToProps = dispatch => {
    return {
        updateContent: async (stateCopy, path, event = null) => {
            event && event.preventDefault()
            if (event?.target.name !== 'name-order') {
                if (stateCopy.currentPage <= 0) {
                    stateCopy.currentPage = 1
                } else if (stateCopy.currentPage > stateCopy.totalPages) {
                    stateCopy.currentPage = stateCopy.totalPages
                }
                const pathArr = path.split('/')
                if (pathArr[1] && !['client', 'professional'].includes(pathArr[1])) {
                    stateCopy.content = [...stateCopy.content]
                    stateCopy.currentIndex = stateCopy.content.indexOf(stateCopy.content.find(entityItem => entityItem['id'] === pathArr[1]))
                    stateCopy.content[stateCopy.currentIndex] = await getRecords(path)
                } else {
                    const data = await getRecords(pathArr[0], stateCopy.globalOrder, stateCopy.printBy, stateCopy.currentPage)
                    if (!data) {
                        !sessionStorage.getItem(AUTH_TOKEN) && ['client', 'procedures', 'results'].includes(path) && new Modal('#loginModal').show()                        
                        return
                    }
                    stateCopy.content = data.content
                    stateCopy.totalPages = Math.ceil(data.total_amount / stateCopy.printBy)
                }
            }
            dispatch(updateContent({...stateCopy
                , content: stateCopy.content
                , currentIndex: stateCopy.currentIndex
                , globalOrder: stateCopy.globalOrder
                , inPageOrder: stateCopy.inPageOrder
                , printBy: stateCopy.printBy
                , totalPages: stateCopy.totalPages
                , currentPage: stateCopy.currentPage
            }))
        }
        , updateDetailed: async (event, path) => {
            event.preventDefault()
            dispatch(updateDetailed(await getRecords(path)))
        }
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(List)

const getPagCoef = props => {
    switch (props.currentPage) {
        case 1:
            return 1
        case 2:
            return 0
        case props.totalPages:
        default:
            return -1
    }
}

const getPagination = (props, pagCoef) => {
    const pagination = []
    pagination.push(props.currentPage - 1 + pagCoef)
    for (let index = -1; index < 2 && index < props.totalPages - 1; index++) {
        pagination.push(props.currentPage + index + pagCoef)
    }
    pagination.push(props.currentPage + 1 + pagCoef)
    return pagination
}