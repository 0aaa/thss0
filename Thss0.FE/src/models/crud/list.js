import React, { Children } from 'react'
import { connect } from 'react-redux'
import { NavLink, useParams } from 'react-router-dom'
import { updateContent } from "../../actionCreator/actionCreator"
import { getRecords } from '../../services/entities'
import { AUTH_TOKEN } from '../../config/consts'
import { UseRedirect, UseUpdate } from '../../config/hook'
import { LOGIN_PATH } from '../../config/consts'

const List = (props) => {
    const params = useParams()
    const isAuthenticated = sessionStorage.getItem(AUTH_TOKEN)
    let path = params.entityName
    if (params.toFind) {
        path = `${params.entityName}/${params.toFind}`
        if (params.entityName !== 'users') {
            path = `search/${path}`
        }
    }
    if (!isAuthenticated && ['users/client', 'procedures', 'results'].includes(path)) {
        UseRedirect(LOGIN_PATH)
        return
    }
    UseUpdate(props, path)
    let pagCoef = 0
    switch (props.state.currentPage) {
        case 1:
            pagCoef = 1
            break;
        case props.state.totalPages:
            pagCoef = -1
            break;
        default:
            pagCoef = 0
    }
    return (
        <>
            {isAuthenticated &&
                <NavLink to={`/add/${params.entityName}`} className="btn btn-outline-primary">Add new</NavLink>
            }
            {props.state.content
                ? <>
                    <label htmlFor="order">Order by name</label>
                    <select id="order" onChange={(event) =>
                                props.updateContent({...props.state, order: event.target.value}, path, event)}
                            className="form-select w-25">
                        {Children.toArray([...Object.entries({true : 'ascendent', false : 'descendent'})].map(e =>
                            <option value={e[0]}>{e[1]}</option>
                        ))}
                    </select>
                    <label htmlFor="print-by">Print by</label>
                    <select id="print-by" onChange={(event) =>
                                props.updateContent({...props.state, printBy: +event.target.value}, path, event)}
                            className="form-select w-25">
                        {Children.toArray([...Array(3).keys()].map(i =>
                            <option value={(i + 1) * 20}>{(i + 1) * 20}</option>
                        ))}
                    </select>
                    <div id="list-error" className="alert alert-danger d-none"></div>
                    <table className="table">
                        <thead>
                            <tr key="thead">
                                <th>
                                    <button name="name-order"
                                            onClick={(event) =>
                                                props.updateContent({...props.state, localOrder: !props.state.localOrder}, path, event)}
                                            className="btn">
                                        Name
                                    </button>
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            {props.state.content.map(cntnt =>
                                <tr id={cntnt.id} key={cntnt.id}>
                                    <td>
                                        <NavLink to={`/details/${params.entityName}/${cntnt.id}`}>
                                            {cntnt.name ?? cntnt.userName ?? cntnt.obtainmentTime}
                                        </NavLink>
                                    </td>
                                    {isAuthenticated &&
                                    <>
                                        <td>
                                            <NavLink to={`/edit/${params.entityName}/${cntnt.id}`}>
                                                Edit
                                            </NavLink>
                                        </td>
                                        <td>
                                            <NavLink to={`/delete/${params.entityName}/${cntnt.id}`}>
                                                Delete
                                            </NavLink>
                                        </td>
                                    </>
                                    }
                                </tr>
                            )}
                        </tbody>
                    </table>
                    {props.state.totalPages > 1 &&
                        <ul className="pagination">
                            {Children.toArray([
                                props.state.currentPage - 1 + pagCoef
                                , props.state.currentPage - 1 + pagCoef
                                , props.state.currentPage + pagCoef
                                , props.state.currentPage + 1 + pagCoef
                                , props.state.currentPage + 1 + pagCoef
                            ].map((pageNum, index) => {
                                switch (index) {
                                    case 0:
                                        return pagCoef !== 1 &&
                                            <li className="page-item">
                                                <button onClick={(event) =>
                                                            props.updateContent({...props.state, currentPage: pageNum - pagCoef}, path, event)}
                                                        className="page-link">
                                                    Back
                                                </button>
                                            </li>
                                    case 4:
                                        return pagCoef !== -1 &&
                                            <li className="page-item">
                                                <button onClick={(event) =>
                                                            props.updateContent({...props.state, currentPage: pageNum - pagCoef}, path, event)}
                                                        className="page-link">
                                                    Forward
                                                </button>
                                            </li>
                                    default:
                                        return <li className="page-item">
                                                <button onClick={(event) =>
                                                            props.updateContent({...props.state, currentPage: pageNum}, path, event)}
                                                        className="page-link">
                                                    {pageNum}
                                                </button>
                                            </li>
                                }
                            }))
                            }
                        </ul>
                    }
                    <a href={`data:application/octet-stream,${encodeURIComponent(JSON.stringify(props.state.content))}`}
                            download={`${Date.now() + params.entityName}.txt`}
                            className="btn btn-outline-primary">
                        Download
                    </a>
                </>
                : <div className="spinner-grow text-primary" role="status">
                    <span className="visually-hidden">Loading...</span>
                </div>
            }
        </>
    )
}

const mapStateToProps = (state) => { return { state } }

const mapDispatchToProps = (dispatch) => {
    return {
        updateContent: async (stateCopy, path, event = null) => {
            event?.preventDefault()
            if (event?.target.name !== 'name-order') {                
                const data = await getRecords(path, stateCopy.order, stateCopy.printBy, stateCopy.currentPage)
                if (!data) {
                    return
                }
                stateCopy.content = data.content
                stateCopy.totalPages = Math.ceil(data.total_amount / stateCopy.printBy)
            }
            dispatch(updateContent(stateCopy.content, stateCopy.totalPages, stateCopy.localOrder))
        }
    }
}
export default connect(mapStateToProps, mapDispatchToProps)(List)