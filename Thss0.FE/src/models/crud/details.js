import React, { Children, useEffect } from 'react'
import { NavLink, useLocation, useNavigate, useParams } from 'react-router-dom'
import { getRecords } from '../../services/entities'
import { connect } from 'react-redux'
import { updateContent } from '../../actionCreator/actionCreator'

function Details(props) {
    const params = useParams()
    const navigate = useNavigate()
    const location = useLocation()
    useEffect(() => { props.updateContent({ ...props }, `${params.entityName}/${params.id}`) }, [location])
    return (
        <div className="vh-100 d-flex flex-column">
            <div id="details-error" className="alert alert-danger d-none"></div>
            {props.content
                ? <div className="h-75">
                    <h5 className="d-flex">{props.content[0]['name'] ?? props.content[0]['obtainmentTime']}
                        <button onClick={() => navigate(-1)} className="btn btn-outline-dark border-0 border-bottom rounded-0 col-2 ms-auto me-2">Back</button>
                    </h5>
                    {Children.toArray(Object.keys(props.content[0]).map(key =>
                        props.content[0][key] !== '' && !key.includes('Names')
                            && <dl>
                                <dt>{key.replace(/([A-Z]+)/g, ' $1').replace(/^./, key[0].toUpperCase())}</dt>
                                <dd>
                                    {props.content[0][key]?.length > 0
                                        ? ['department', 'user', 'procedure', 'result', 'substance'].includes(key)
                                            ? Children.toArray(props.content[0][key].split('\n').filter(e => e !== '').map((e, i) =>
                                                <>
                                                    <NavLink to={`/details/${key}s/${e}`}>
                                                        {props.content[0][key + 'Names'].split('\n')[i]}
                                                    </NavLink>
                                                    <br />
                                                </>))
                                            : props.content[0][key]
                                        : 'Empty'
                                    }
                                </dd>
                            </dl>
                    ))}
                </div>
                : <div className="h-75 d-flex justify-content-center align-items-center gap-1">
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

const mapStateToProps = (state) => { return state }

const mapDispatchToProps = (dispatch) => {
    return {
        updateContent: async (stateCopy, path) => {
            const data = await getRecords(path)
            if (data) {
                delete data['id']
                dispatch(updateContent([data], stateCopy.totalPages, stateCopy.localOrder, stateCopy.currentPage))
            }
        }
    }
}
export default connect(mapStateToProps, mapDispatchToProps)(Details)