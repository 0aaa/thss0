import React, { Children } from 'react'
import { connect } from 'react-redux'
import { useNavigate, useParams } from 'react-router-dom'
import { updateContent, updateDetailed } from '../../../actionCreator/actionCreator'
import { getRecord, getRecords } from '../../../services/entities'
import { AUTH_TOKEN } from '../../../config/consts'
import { UseRedirect, UseUpdate } from '../../../config/hooks'
import { HOME_PATH } from '../../../config/consts'
import { Modal } from 'bootstrap'
import Offcanvas from '../../structural/offcanvas'

const List = props => {
    const params = useParams()
    const navigate = useNavigate()
    const isAuthenticated = sessionStorage.getItem(AUTH_TOKEN)
    if (!isAuthenticated && ['client', 'results'].includes(params.entityName)) {
        UseRedirect(HOME_PATH)
    }
    UseUpdate(props, (params.toFind && `search/${encodeURIComponent(params.toFind)}/${params.entityName}`) || params.entityName)
    const pagCoef = getPagCoef(props)
    const pagination = getPagination(props, pagCoef)
    return <div className="vh-100">
            <h4 className="float-start">{params.entityName.replace(/^./, params.entityName[0].toUpperCase())}</h4>
            {(props.content
                && <>
                    <div className="btn-group w-75 d-flex flex-wrap ms-auto me-2">
                        {isAuthenticated
                            && <button onClick={event => props.updateDetailed(event, `${params.entityName}/${props.content[0]?.id}`, 'Add')} className={`btn btn-outline-${props.btnColor} border-0 border-bottom rounded-0 px-0`} data-bs-toggle="offcanvas" data-bs-target="#offcanvasCrud" aria-controls="offcanvasCrud">Add new</button>
                        }
                        <a href={`data:application/octet-stream,${encodeURIComponent(JSON.stringify(props.content))}`}
                                download={`${Date.now() + params.entityName}.txt`}
                                className={`btn btn-outline-${props.btnColor} border-0 border-bottom px-0`}>
                            Download
                        </a>
                        {params.entityName === 'search'
                            && <select onChange={event => { params.entityName = event.target.value; props.updateContent({...props}, `search/${encodeURIComponent(params.toFind)}/${event.target.value}`, event) }}
                                        defaultValue="Category"
                                        className={`btn btn-outline-${props.btnColor} border-0 border-bottom rounded-0 px-0`}>
                                    <option disabled hidden>Category</option>
                                    {Children.toArray(['departments', 'clients', 'professionals', 'procedures', 'results', 'substances'].map(e =>
                                        <option value={e}>{e.replace(/^./, e[0].toUpperCase())}</option>
                                    ))}
                            </select>
                        }
                        <select id="order" onChange={event =>
                                props.updateContent({...props, globalOrder: event.target.value}, params.entityName, event)}
                                defaultValue="Order"
                                className={`btn btn-outline-${props.btnColor} border-0 border-bottom px-0`}>
                            <option disabled hidden>Order</option>
                            {Children.toArray(['ascendent', 'descendent'].map(globalOrder =>
                                <option value={globalOrder === 'ascendent'}>{globalOrder}</option>
                            ))}
                        </select>
                        <select id="printBy" onChange={event =>
                                props.updateContent({...props, printBy: +event.target.value}, params.entityName, event)}
                                defaultValue="Print by"
                                className={`btn btn-outline-${props.btnColor} border-0 border-bottom rounded-0 px-0`}>
                            <option disabled hidden>Print by</option>
                            {Children.toArray([...Array(3).keys()].map(i =>
                                <option value={(i + 1) * 20}>{(i + 1) * 20}</option>
                            ))}
                        </select>
                        <button onClick={() => navigate(-1)} className={`btn btn-outline-${props.btnColor} border-0 border-bottom rounded-0 px-0`}>Back</button>
                    </div>
                    <div className="d-flex flex-column">
                        <div id="listError" className="alert alert-danger d-none"></div>
                        <table className="table">
                            <thead>
                                <tr>
                                    <th className="p-0">
                                        <button name="nameOrder"
                                                onClick={event => props.updateInPageOrder(event, {...props, inPageOrder: !props.inPageOrder})}
                                                className={`btn btn-outline-${props.btnColor} border-0 rounded-0 w-100 text-start p-3`}>
                                            Name {(props.inPageOrder && <>&darr;</>) || <>&uarr;</>}
                                        </button>
                                    </th>
                                    {isAuthenticated
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
                                            <button onClick={event => props.updateDetailed(event, `${params.entityName}/${entityItem.id}`, 'Details')} className={`btn btn-outline-${props.btnColor} border-0 rounded-0 w-100 text-start p-3`} data-bs-toggle="offcanvas" data-bs-target="#offcanvasCrud" aria-controls="offcanvasCrud">
                                                {entityItem.name}
                                            </button>
                                        </td>
                                        {isAuthenticated
                                            && <>
                                                <td className="p-0">
                                                    <button onClick={event => props.updateDetailed(event, `${params.entityName}/${entityItem.id}`, 'Edit')} className={`btn btn-outline-${props.btnColor} border-0 rounded-0 w-100 text-start p-3`} data-bs-toggle="offcanvas" data-bs-target="#offcanvasCrud" aria-controls="offcanvasCrud">
                                                        Edit
                                                    </button>
                                                </td>
                                                <td className="p-0">
                                                    <button onClick={event => props.updateDetailed(event, `${params.entityName}/${entityItem.id}`, 'Delete')} className={`btn btn-outline-${props.btnColor} border-0 rounded-0 w-100 text-start p-3`} data-bs-toggle="offcanvas" data-bs-target="#offcanvasCrud" aria-controls="offcanvasCrud">
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
                                {Children.toArray(pagination.map((pageNum, index) => {
                                        switch (index) {
                                            case 0:
                                                return props.currentPage !== 1
                                                    && <li className="page-item">
                                                        <button onClick={event => props.updateContent({...props, currentPage: pageNum - pagCoef}, params.entityName, event)}
                                                                className="page-link bg-dark text-white rounded-0">
                                                            Back
                                                        </button>
                                                    </li>
                                            case pagination.length - 1:
                                                return props.currentPage !== props.totalPages
                                                    && <li className="page-item">
                                                        <button onClick={event => props.updateContent({...props, currentPage: pageNum - pagCoef}, params.entityName, event)}
                                                                className="page-link bg-dark text-white rounded-0">
                                                            Forward
                                                        </button>
                                                    </li>
                                            default:
                                                return <li className="page-item">
                                                        <button onClick={event => props.updateContent({...props, currentPage: pageNum}, params.entityName, event)}
                                                                className="page-link bg-dark text-white rounded-0">
                                                            {pageNum.toString()}
                                                        </button>
                                                    </li>
                                        }
                                    }))
                                }
                            </ul>
                        }
                    </div>
                </>)
                || <div className="h-75 d-flex justify-content-center align-items-center gap-1">
                    {Children.toArray([...Array(3).keys()].map(() =>
                        <div className="spinner-grow text-primary" role="status">
                            <span className="visually-hidden">Loading...</span>
                        </div>
                    ))}
                </div>
            }
            <Offcanvas entityName={params.entityName} />
        </div>
}

const mapStateToProps = state => state

const mapDispatchToProps = dispatch => ({
    updateContent: async (stateCopy, path, event = null) => {
        event && event.preventDefault()
        setCurrentPage(stateCopy)
        const data = await getRecords(path, stateCopy.printBy, stateCopy.currentPage, stateCopy.globalOrder)
        if (!data) {
            !sessionStorage.getItem(AUTH_TOKEN) && ['client', 'results'].includes(path) && new Modal('#loginModal').show()
            return
        }
        stateCopy.content = data.content
        stateCopy.totalPages = Math.ceil(data.totalAmount / stateCopy.printBy)
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
    , updateInPageOrder: (event, stateCopy) => {
        event.preventDefault()
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
    , updateDetailed: async (event, path, offcanvasName) => {
        event.preventDefault()
        dispatch(updateDetailed(await getRecord(path), offcanvasName))
    }
})

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

const setCurrentPage = stateCopy => {
    if (stateCopy.currentPage <= 0) {
        stateCopy.currentPage = 1
    } else if (stateCopy.currentPage > stateCopy.totalPages) {
        stateCopy.currentPage = stateCopy.totalPages
    }
}
