import React, { Children } from 'react'
import { connect } from 'react-redux'
import { getRecord } from '../../../services/entities'
import { updateDetailed } from '../../../actionCreator/actionCreator'

const Details = props =>
    <div className="offcanvas-body">
        {(props.detailedItem
            && <>
                {props.detailedItem['photo'] && <img src={`data:image/jpeg;base64, ${props.detailedItem['photo']}`} style={{ height: '320px' }} />}
                {Children.toArray(Object.keys(props.detailedItem).reverse().map(key =>
                    !(key.includes('Names') || props.detailedItem[key] === '' || key === 'photo')
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
                            {key === 'content' && <button onClick={event => props.updateDetailed(event, `OpenAI/${props.detailedItem[key]}`)} className="btn btn-outline-dark border-0 rounded-0 w-100 text-start p-3">AI Analyse</button>}
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
    </div>

const mapStateToProps = state => ({ detailedItem: state.detailedItem })

const mapDispatchToProps = dispatch => ({
    updateDetailed: async (event, path) => {
        event.preventDefault()
        dispatch(updateDetailed(await getRecord(path), 'Details'))
    }    
})

export default connect(mapStateToProps, mapDispatchToProps)(Details)