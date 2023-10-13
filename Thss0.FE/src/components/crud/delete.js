import React, { Children } from 'react'
import { connect } from 'react-redux'
import { updateContent } from '../../actionCreator/actionCreator'
import { deleteRecord, getRecords } from '../../services/entities'
import { Offcanvas } from 'bootstrap'

const Delete = props =>
    props.detailedItem
        && <div className="offcanvas offcanvas-end" data-bs-scroll="true" tabIndex="-1" id="offcanvasDelete" aria-labelledby="offcanvasDeleteLabel">
            <div className="offcanvas-header">
                <h5 className="offcanvas-title" id="offcanvasDeleteLabel">Delete {props.detailedItem['name'] ?? props.detailedItem['obtainmentTime']}</h5>
                <button className="btn-close" data-bs-dismiss="offcanvas" aria-label="Close"></button>
                <div id="delete-error" className="alert alert-danger d-none"></div>
            </div>
            <form onSubmit={event => props.handleDelete(event, {...props})} className="offcanvas-body d-flex flex-column h-100">
                {Children.toArray(Object.keys(props.detailedItem).map(key =>
                    props.detailedItem[key] !== '' && !key.includes('Names')
                    && <dl>
                        <dt>{key.replace(/([A-Z]+)/g, ' $1').replace(/^./, key[0].toUpperCase())}</dt>
                        <dd>
                            {props.detailedItem[key].length > 0
                                ? (['department', 'user', 'procedure', 'result'].includes(key)
                                    ? Children.toArray(props.detailedItem[key].split('\n').filter(e => e !== '').map((_, i) =>
                                        <>
                                            {props.detailedItem[`${key}Names`].split('\n')[i]}
                                            <br />
                                        </>))
                                    : props.detailedItem[key])
                                : 'Empty'
                            }
                        </dd>
                    </dl>
                ))}
                <div className="btn-group w-100 mt-auto">
                    <input type="submit" value="Delete" className="btn btn-outline-danger border-0 border-bottom rounded-0 col-6" data-bs-dismiss="offcanvas" />
                    <button className="btn btn-outline-dark border-0 border-bottom rounded-0 col-6" data-bs-dismiss="offcanvas" aria-label="Close">Cancel</button>
                </div>
            </form>
        </div>

const mapStateToProps = state => state

const mapDispatchToProps = dispatch => {
    return {
        handleDelete: async (event, stateCopy) => {
            event.preventDefault()
            Offcanvas.getInstance('#offcanvasDelete').hide()
            await deleteRecord(`${stateCopy.entityName}/${stateCopy.detailedItem['id']}`)
            const data = await getRecords(stateCopy.entityName)
            data && dispatch(updateContent({...stateCopy, content: data.content}))
        }
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(Delete)