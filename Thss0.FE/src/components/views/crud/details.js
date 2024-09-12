import React, { Children } from 'react'
import { connect } from 'react-redux'
import { getRecord } from '../../../services/entities'
import { updateDetailed, updateModal } from '../../../actionCreator/actionCreator'
import { AUTH_TOKEN } from '../../../config/consts'

const Details = props => {
    const isAuthenticated = sessionStorage.getItem(AUTH_TOKEN)
    return <div className="offcanvas-body">
        {(props.detailedItem
            && <>
            {props.detailedItem['photo'] && <img src={`data:image/jpg;base64, ${btoa(String.fromCharCode.apply(null, new Uint8Array(props.detailedItem['photo'])))}`} alt="" className="w-50" />}
                {Children.toArray(Object.keys(props.detailedItem).reverse().map(k =>
                    !(k.includes('Names') || props.detailedItem[k] === '' || k === 'photo')
                    && <dl className="bg-white bg-opacity-50 p-2">
                        <dt className="fw-semibold">{k.replace(/([A-Z]+)/g, ' $1').replace(/^./, k[0].toUpperCase())}</dt>
                        <dd>
                            {(['department', 'client', 'professional', 'procedure', 'result', 'substance'].includes(k)
                                && Children.toArray(props.detailedItem[k].split('\n').filter(e => e !== '').map((id, i) =>
                                    <button onClick={e => props.updateDetailed(e, `${k}/${id}`)} className={`btn btn-outline-dark border-0 rounded-0 w-100 text-start ${(!isAuthenticated && 'disabled') || ''}`} data-bs-target="#offcanvasDetails">
                                        {props.detailedItem[`${k}Names`].split('\n')[i]}
                                    </button>)))
                                || props.detailedItem[k]
                            }
                        </dd>
                            {k === 'content' && <button onClick={e => props.updateModal(e, `OpenAI/${props.detailedItem[k]}`)} className="btn btn-outline-dark border-0 rounded-0 w-100 text-start p-3"
                                data-bs-toggle="modal" data-bs-target="#modalGen">AI Analyse</button>}
                    </dl>
                ))}
            </>)
            || <div className="h-75 d-flex justify-content-center align-items-center gap-1">
                {Children.toArray([...Array(3).keys()].map(() =>
                    <div className="spinner-grow text-primary" role="status">
                        <span className="visually-hidden">Loading...</span>
                    </div>
                ))}
            </div>
        }
    </div>}

const mapStateToProps = state => ({ detailedItem: state.detailedItem })

const mapDispatchToProps = dispatch => ({
    updateDetailed: async (e, path) => {
        e.preventDefault()
        dispatch(updateDetailed(await getRecord(path), 'Details'))
    }
    , updateModal: async (e, path) => {
        e.preventDefault()
        dispatch(updateModal(e.target.innerHTML, await getRecord(path)))
    }
})

export default connect(mapStateToProps, mapDispatchToProps)(Details)