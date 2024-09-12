import { Children } from 'react'
import { getRecords } from '../../services/entities'
import { connect } from 'react-redux'
import { updateContent } from '../../actionCreator/actionCreator'
import { UseUpdate } from '../../config/hooks'

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
        <select onChange={e => props.updateContent({ ...props, order: e.target.value }, path, e)}
                defaultValue="Department"
                className="btn btn-outline-dark border-0 border-bottom rounded-0">
            <option disabled hidden>Department</option>
            {Children.toArray(props.content.map(d =>
                <option value={d['id']}>{d['name']}</option>
            ))}
        </select>
        <div id="listError" className="alert alert-danger d-none"></div>
        <table className="table">
            <thead>
                <tr>
                    <th>Name</th>
                    {[...Array(timeLinePts).keys()].map(k => <th key={`th-${k}`} className={(k % 4 && ' ') || 'fs-5'}>{k % 4 * 15 || (k / 4 < 10 && '0') + k / 4}</th>)}
                </tr>
            </thead>
            <tbody onMouseDown={e => selectCell(e, sourceCellIndex)} onMouseOver={e => hoverCell(e, sourceCellIndex, timeLinePts)} onMouseUp={() => sourceCellIndex = null}>
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

const mapDispatchToProps = dispatch => ({
    updateContent: async (stateCopy, path) => {
        const professionals = (await getRecords(path)).content
        const procByProf = []
        let procedureIds = []
        for (let i = 0; i < professionals.length; i++) {
            procByProf.push({ userName: professionals[i]['userName'], procedures: [] })
            procedureIds = (await getRecords(`users/${professionals[i]['id']}`))['procedure'].split('\n')
            for (let jndex = 0; jndex < procedureIds.length && procedureIds[jndex] !== ''; jndex++) {
                procByProf[i]['procedures'].push(await getRecords(`procedures/${procedureIds[jndex]}`))
                procByProf[i]['procedures'][jndex]['id'] = procedureIds[jndex]
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
})

export default connect(mapStateToProps, mapDispatchToProps)(Schedule)

const selectCell = (e, sourceCellIndex) => {
    e.preventDefault()
    sourceCellIndex = [...e.target.parentNode.childNodes].indexOf(e.target)
    e.target.className = (e.target.className === '' && 'bg-danger') || ''
}

const hoverCell = (e, sourceCellIndex, timeLinePts) => {
    if (!sourceCellIndex) {
        return
    }
    const curCellIndex = [...e.target.parentNode.childNodes].indexOf(e.target)
    e.target.className = e.target.parentNode.childNodes[curCellIndex
            + ((curCellIndex > 1 && sourceCellIndex < curCellIndex && -1) || (curCellIndex !== timeLinePts && sourceCellIndex > curCellIndex))]
        .className
}