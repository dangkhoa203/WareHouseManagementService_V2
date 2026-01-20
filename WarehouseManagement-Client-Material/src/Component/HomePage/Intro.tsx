import {useNavigate, useOutletContext} from "react-router";
import Container from "@mui/material/Container";
import Button from "@mui/material/Button";
import {Card, CardContent, Grid2} from "@mui/material";
import Typography from "@mui/material/Typography";
import PhonelinkTwoToneIcon from '@mui/icons-material/PhonelinkTwoTone';
import SettingsSuggestTwoToneIcon from '@mui/icons-material/SettingsSuggestTwoTone';
import VerifiedUserTwoToneIcon from '@mui/icons-material/VerifiedUserTwoTone';
import WalletTwoToneIcon from '@mui/icons-material/WalletTwoTone';
import PeopleAltTwoToneIcon from '@mui/icons-material/PeopleAltTwoTone';
import userInfo from "../../Type/userInfo.tsx";
import {useUserInfo} from "../../State/User.ts";

export default function Intro(){
    const userInfo = useUserInfo((state)=> state.user);
    const navigate = useNavigate();
    return(
        <Container maxWidth="xl" style={{minHeight: "100vh",padding:"0"}}>
            <Container maxWidth="xl" id="Hero">
                <div id="Hero-Text">
                    <p style={{color:"#E78B48"}}>Quản lý cửa hàng của bạn</p>
                    <p>Không bao giờ dễ hơn như trước</p>
                </div>
                <div id="Hero-Action">
                    <p>Hãy đăng ký tài khoản và bắt đầu quản lý</p>
                    <Button color="warning" variant="contained" onClick={()=>{navigate(userInfo.isLoggedIn ? "Workspace":"Register")}} size="large" >Bắt đầu</Button>
                </div>
            </Container>
            <Container sx={{paddingY:"20px"}} maxWidth="lg">
                <Grid2 sx={{
                    justifyContent: "center",
                    alignItems: "center",
                }} container spacing={2}>
                    <Grid2 sx={{textAlign:"center"}} size={12}>
                        <h2>Tại sao phải chọn chúng tôi</h2>
                    </Grid2>
                    <Grid2 size={{xs:12,md:6,lg:4}}>
                        <Card sx={{textAlign:"center",borderTop:"5px solid #E78B48",minHeight:"350px"}}>
                            <CardContent>
                                <Typography variant="h5" component="div">
                                    <VerifiedUserTwoToneIcon sx={{ fontSize: 100,color:"#E78B48" }} />
                                </Typography>
                                <Typography sx={{ color: 'text.secondary',fontSize:"2em", mb: 1.5 }}>Bảo mật</Typography>
                                <Typography sx={{textAlign:"center"}}>
                                    Hệ thống của chúng tôi được xây dựng với các biện pháp bảo mật tiên tiến nhất, bao gồm mã hóa dữ liệu đa tầng, kiểm soát truy cập nghiêm ngặt và sao lưu thường xuyên.
                                </Typography>
                            </CardContent>
                        </Card>
                    </Grid2>
                    <Grid2 size={{xs:12,md:6,lg:4}}>
                        <Card sx={{textAlign:"center",borderTop:"5px solid #E78B48",minHeight:"350px"}}>
                            <CardContent>
                                <Typography variant="h5" component="div">
                                    <WalletTwoToneIcon sx={{ fontSize: 100,color:"#E78B48" }} />
                                </Typography>
                                <Typography sx={{ color: 'text.secondary',fontSize:"2em", mb: 1.5 }}>Ít chi phí</Typography>
                                <Typography sx={{textAlign:"center"}}>
                                    Chúng tôi cam kết cung cấp một giải pháp quản lý kho mạnh mẽ với mức chi phí cạnh tranh nhất trên thị trường.
                                </Typography>
                            </CardContent>
                        </Card>
                    </Grid2>
                    <Grid2 size={{xs:12,md:6,lg:4}}>
                        <Card sx={{textAlign:"center",borderTop:"5px solid #E78B48",minHeight:"350px"}}>
                            <CardContent>
                                <Typography variant="h5" component="div">
                                    <PhonelinkTwoToneIcon sx={{ fontSize: 100,color:"#E78B48" }} />
                                </Typography>
                                <Typography sx={{ color: 'text.secondary',fontSize:"2em", mb: 1.5 }}>Tiện lợi</Typography>
                                <Typography sx={{textAlign:"center"}}>
                                    Với giao diện trực quan và dễ sử dụng, website của chúng tôi cho phép Quý khách quản lý kho mọi lúc mọi nơi, trên mọi thiết bị có kết nối internet.
                                </Typography>
                            </CardContent>
                        </Card>
                    </Grid2>
                    <Grid2 size={{xs:12,md:6,lg:4}}>
                        <Card sx={{textAlign:"center",borderTop:"5px solid #E78B48",minHeight:"350px"}}>
                            <CardContent>
                                <Typography variant="h5" component="div">
                                    <SettingsSuggestTwoToneIcon sx={{ fontSize: 100,color:"#E78B48" }} />
                                </Typography>
                                <Typography sx={{ color: 'text.secondary',fontSize:"2em", mb: 1.5 }}>Hiệu quả</Typography>
                                <Typography sx={{textAlign:"center"}}>
                                    Hệ thống quản lý kho thông minh của chúng tôi cung cấp các công cụ phân tích và báo cáo chi tiết
                                    giúp Quý khách nắm bắt chính xác tình hình kho.
                                </Typography>
                            </CardContent>
                        </Card>
                    </Grid2>
                    <Grid2 size={{xs:12,md:6,lg:4}}>
                        <Card sx={{textAlign:"center",borderTop:"5px solid #E78B48",minHeight:"350px"}}>
                            <CardContent>
                                <Typography variant="h5" component="div">
                                    <PeopleAltTwoToneIcon sx={{ fontSize: 100,color:"#E78B48" }} />
                                </Typography>
                                <Typography sx={{ color: 'text.secondary',fontSize:"2em", mb: 1.5 }}>Hiệu quả</Typography>
                                <Typography sx={{textAlign:"center"}}>
                                    Chúng tôi cung cấp khả năng tạo và quản lý nhiều tài khoản nhân viên với các quyền truy cập khác nhau, giúp phân quyền quản lý rõ ràng, nâng cao tính trách nhiệm và hiệu quả làm việc của từng bộ phận.
                                </Typography>
                            </CardContent>
                        </Card>
                    </Grid2>
                </Grid2>
            </Container>
        </Container>
    )
}