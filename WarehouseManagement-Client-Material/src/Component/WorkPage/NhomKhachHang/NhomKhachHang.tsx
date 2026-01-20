import {useEffect, useState} from "react";
import { ColDef } from 'ag-grid-community';
import Container from "@mui/material/Container";
import {AgGridReact} from "ag-grid-react";
import {AG_GRID_LOCALE_VN} from "@ag-grid-community/locale";
import {myTheme} from "../../../Type/myTheme.tsx";
import Button from "@mui/material/Button";
import {useNavigate} from "react-router";
interface customerGroupData{
    id: string,
    name: string,
    dateCreated:Date
}
export default function NhomKhachHang(){
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [err, setError] = useState(false);
    const [rowData, setRowData] = useState<Array<customerGroupData>>([]);
    const getData = async () =>{
        try {
            setLoading(true);
            setError(false);
            const response = await fetch('https://localhost:7075/api/Customer-Groups', {
                headers: { 'Content-Type': 'application/json' },
                credentials: 'include',
                method:"GET"
            });
            if (!response.ok) {
                setError(true);
            }
            const content = await response.json();
            setRowData(content.data);
        }catch (er){
            setError(true);
        }finally {
            setLoading(false);
        }
    }
    const [colDefs, setColDefs] = useState<ColDef[]>([
        { field: "id",headerName:"ID",filter:true,resizable:false,unSortIcon: true,flex: 1,minWidth:200,floatingFilter: true },
        { field: "name",headerName:"Tên",filter:true,resizable:false,unSortIcon: true,flex: 1,minWidth:200,floatingFilter: true },
        { field: "dateCreated",headerName:"Ngày tạo dữ liệu",filter:true,resizable:false,unSortIcon: true,flex: 1,minWidth:200,floatingFilter: true },
    ]);
    useEffect(() => {
        (async ()=> await getData())()
    }, []);
    return(
        <Container sx={{display:"flex", flexDirection:"column", justifyContent:"center",gap:2}}>
            <p>Danh sách nhóm khách hàng</p>
            <Button variant="contained" sx={{width: {md:"150px",lg:"150px"}}} onClick={()=>navigate("Tao")}>Tạo</Button>
            <div style={{ height: "400px" }}>
                <AgGridReact
                    rowData={rowData}
                    columnDefs={colDefs}
                    theme={myTheme}
                    pagination={true}
                    localeText={AG_GRID_LOCALE_VN}
                />
            </div>
        </Container>
    )
}