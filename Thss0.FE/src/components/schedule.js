import { Children } from 'react'
import { getRecords } from '../services/entities'
import { connect } from 'react-redux'
import { updateContent } from '../actionCreator/actionCreator'
import { UseUpdate } from '../config/hooks'

const Schedule = props => {
    // const path = `/departments/${params.id}`
    const path = `professional/true/20/1`// Test.
    const timeLinePts = 97
    UseUpdate(props, path)
    if (!(props.content[0] && props.content[0]['procedures'])) {
        return
    }
    let sourceCellIndex
    return <>
        <select onChange={event =>
            props.updateContent({ ...props, order: event.target.value }, path, event)}
            defaultValue="Department"
            className="btn btn-outline-dark border-0 border-bottom rounded-0">
            <option disabled hidden>Department</option>
            {Children.toArray(props.content.map(d =>
                <option value={d['id']}>{d['name']}</option>
            ))}
        </select>
        <div id="list-error" className="alert alert-danger d-none"></div>
        <table className="table">
            <thead>
                <tr>
                    <th>Name</th>
                    {[...Array(timeLinePts).keys()].map(k => <th key={`th-${k}`} className={(k % 4 && ' ') || 'fs-5'}>{k % 4 * 15 || (k / 4 < 10 && '0') + k / 4}</th>)}
                </tr>
            </thead>
            <tbody onMouseDown={event => selectCell(event, sourceCellIndex)} onMouseOver={event => hoverCell(event, sourceCellIndex, timeLinePts)} onMouseUp={() => sourceCellIndex = null}>
                {Children.toArray(props.content.map((professional, i) =>
                    <tr>
                        <td>{professional.userName}</td>
                        {[...Array(timeLinePts).keys()].map(k =>
                            <td key={`td-${i}-${k}`}
                                className={(professional.procedures.some(p => {
                                    const bT = p.beginTime.split(/[\s:]/)
                                    const eT = p.endTime.split(/[\s:]/)
                                    return bT[1] <= k / 4 && bT[2] <= k % 4 * 15 && (eT[1] > k / 4 || (+eT[1] === Math.floor(k / 4) && eT[2] > k % 4 * 15))
                                }) && 'bg-danger') || ''}>
                            </td>
                        )}
                    </tr>
                ))}
            </tbody>
        </table>
    </>
}

const mapStateToProps = state => state

const mapDispatchToProps = dispatch => {
    return {
        updateContent: async (stateCopy, path) => {
            const professionals = (await getRecords(path)).content
            const procByProf = []
            let procedureIds = []
            for (let index = 0; index < professionals.length; index++) {
                procByProf.push({ userName: professionals[index]['userName'], procedures: [] })
                procedureIds = (await getRecords(`users/${professionals[index]['id']}`))['procedure'].split('\n')
                for (let jndex = 0; jndex < procedureIds.length && procedureIds[jndex] !== ''; jndex++) {
                    procByProf[index]['procedures'].push(await getRecords(`procedures/${procedureIds[jndex]}`))
                    procByProf[index]['procedures'][jndex]['id'] = procedureIds[jndex]
                }
            }
            if (procByProf) {
                dispatch(updateContent({...stateCopy
                    , content: procByProf
                    , currentIndex: stateCopy.currentIndex
                    , globalOrder: stateCopy.globalOrder
                    , inPageOrder: stateCopy.inPageOrder
                    , printBy: stateCopy.printBy
                    , totalPages: stateCopy.totalPages
                    , currentPage: stateCopy.currentPage
                }))
            }
        }
    }
}
export default connect(mapStateToProps, mapDispatchToProps)(Schedule)

const selectCell = (event, sourceCellIndex) => {
    event.preventDefault()
    sourceCellIndex = [...event.target.parentNode.childNodes].indexOf(event.target)
    event.target.className = (event.target.className === '' && 'bg-danger') || ''
}

const hoverCell = (event, sourceCellIndex, timeLinePts) => {
    if (!sourceCellIndex) {
        return
    }
    const curCellIndex = [...event.target.parentNode.childNodes].indexOf(event.target)
    event.target.className = event.target.parentNode.childNodes[curCellIndex
            + ((curCellIndex > 1 && sourceCellIndex < curCellIndex && -1) || (curCellIndex !== timeLinePts && sourceCellIndex > curCellIndex))]
        .className
}