import React, { Children } from 'react'
import { connect } from 'react-redux'
import { updateContent } from '../../../actionCreator/actionCreator'
import { deleteRecord, getRecords } from '../../../services/entities'
import { Offcanvas } from 'bootstrap'

const Delete = props =>
    props.detailedItem
    && <form onSubmit={e => props.handleDelete(e, { ...props })} className="offcanvas-body d-flex flex-column h-100">
        {props.detailedItem['photo'] && <img src={`data:image/jpg;base64, ${props.detailedItem['photo']}`} alt="" className="w-50" />}
        {Children.toArray(Object.keys(props.detailedItem).map(k =>
            props.detailedItem[k] !== '' && !k.includes('Names') && k !== 'photo'
            && <dl>
                <dt>{k.replace(/([A-Z]+)/g, ' $1').replace(/^./, k[0].toUpperCase())}</dt>
                <dd>
                    {props.detailedItem[k].length > 0
                        ? (['department', 'client', 'professional', 'procedure', 'result'].includes(k)
                            ? Children.toArray(props.detailedItem[k].split('\n').filter(e => e !== '').map((_, i) =>
                                <>
                                    {props.detailedItem[`${k}Names`].split('\n')[i]}
                                    <br />
                                </>))
                            : props.detailedItem[k])
                        : 'Empty'
                    }
                </dd>
            </dl>
        ))}
        <div className="btn-group w-100 mt-auto">
            <input type="submit" value="Delete" className="btn btn-outline-danger border-0 border-bottom rounded-0 col-6" data-bs-dismiss="offcanvas" />
            <button type="button" className="btn btn-outline-dark border-0 border-bottom rounded-0 col-6" data-bs-dismiss="offcanvas">Cancel</button>
        </div>
    </form>

const mapStateToProps = state => state

const mapDispatchToProps = dispatch => ({
    handleDelete: async (e, stateCopy) => {
        e.preventDefault()
        Offcanvas.getInstance('#crud').hide()
        await deleteRecord(`${stateCopy.entityName}/${stateCopy.detailedItem['id']}`)
        const data = await getRecords(stateCopy.entityName)
        data && dispatch(updateContent({ ...stateCopy, content: data.content }))
    }
})

export default connect(mapStateToProps, mapDispatchToProps)(Delete)