import React, { Children } from 'react'
import { connect } from 'react-redux'
import { getRecords } from '../../services/entities'
import { updateDetailed } from '../../actionCreator/actionCreator'

const Details = props =>
    props.detailedItem
        && <div className="offcanvas offcanvas-end" data-bs-scroll="true" tabIndex="-1" id="offcanvasDetails" aria-labelledby="offcanvasDetailsLabel">
            <div className="offcanvas-header">
                <h5 className="offcanvas-title" id="offcanvasDetailsLabel">{props.detailedItem['name'] ?? props.detailedItem['obtainmentTime']}</h5>
                <button className="btn-close" data-bs-dismiss="offcanvas" aria-label="Close"></button>
                <div id="details-error" className="alert alert-danger d-none"></div>
            </div>
            <div className="offcanvas-body">
                {(props.detailedItem
                    && Children.toArray(Object.keys(props.detailedItem).map(key =>
                        !(key.includes('Names') || props.detailedItem[key] === '')
                            && <dl>
                                <dt>{key.replace(/([A-Z]+)/g, ' $1').replace(/^./, key[0].toUpperCase())}</dt>
                                <dd>
                                    {(['department', 'user', 'procedure', 'result', 'substance'].includes(key)
                                        && Children.toArray(props.detailedItem[key].split('\n').filter(e => e !== '').map((id, i) =>
                                            <button onClick={event => props.updateDetailed(event, `${key}s/${id}`)} className="btn btn-outline-dark border-0 rounded-0 w-100 text-start p-3" data-bs-target="#offcanvasDetails" aria-controls="offcanvasDetails">
                                                {props.detailedItem[`${key}Names`].split('\n')[i]}
                                            </button>)))
                                        || props.detailedItem[key]
                                    }
                                </dd>
                            </dl>
                        ))
                    )
                    || <div className="h-75 d-flex justify-content-center align-items-center gap-1">
                        {Children.toArray([...Array(3).keys()].map(() =>
                            <div className="spinner-grow text-primary" role="status">
                                <span className="visually-hidden">Loading...</span>
                            </div>
                        ))}
                    </div>
                }
            </div>
        </div>

const mapStateToProps = state => {
    return { detailedItem: state.detailedItem }
}

const mapDispatchToProps = dispatch => {
    return {
        updateDetailed: async (event, path) => {
            event.preventDefault()
            dispatch(updateDetailed(await getRecords(path)))
        }
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(Details)