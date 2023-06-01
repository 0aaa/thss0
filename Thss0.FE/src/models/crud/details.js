import React, { useEffect } from 'react'
import { NavLink, useLocation, useNavigate, useParams } from 'react-router-dom'
import { getRecords } from '../../services/entities'
import { connect } from 'react-redux'
import { updateContent } from '../../actionCreator/actionCreator'

function Details(props) {
    const params = useParams()
    const navigate = useNavigate()
    const location = useLocation()
    useEffect(() => { props.updateContent({...props.state}, `${params.entityName}/${params.id}`) }, [location])
    console.log(props.state.content[0])
    return (
        <>
            <div id="details-error" className="alert alert-danger d-none"></div>
            <h5>{props.state.content[0]['name']}</h5>
            {Object.keys(props.state.content[0]).map(key =>
                props.state.content[0][key] !== '' && !key.includes('Names') &&
                <dl key={'dl-' + key}>
                    <dt>{key.replace(/([A-Z]+)/g, ' $1').replace(/^./, key[0].toUpperCase())}</dt>
                    <dd>
                        {props.state.content[0][key]?.length > 0
                            ? ['department', 'user', 'procedure', 'result'].includes(key)
                                ? props.state.content[0][key].split('\n').filter(e => e !== '').map((e, i) =>
                                    <>
                                        <NavLink key={'a-' + key} to={`/details/${key}s/${e}`}>
                                            {props.state.content[0][key + 'Names'].split('\n')[i]}
                                        </NavLink>
                                        <br/>
                                    </>)
                                : props.state.content[0][key]
                            : 'Empty'
                        }
                    </dd>
                </dl>
            )}
            <button onClick={() => navigate(-1)} className="btn btn-outline-primary">Back</button>
        </>
    )
}

const mapStateToProps = (state) => { return { state } }

const mapDispatchToProps = (dispatch) => {
    return {
        updateContent: async (stateCopy, path) => {      
            const data = await getRecords(path)
            delete data['id']
            dispatch(updateContent([data], stateCopy.totalPages, stateCopy.localOrder))
        }
    }
}
export default connect(mapStateToProps, mapDispatchToProps)(Details)