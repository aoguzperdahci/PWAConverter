import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SpeedDialModule } from 'primeng/speeddial';
import { ToastModule } from 'primeng/toast';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AvatarModule } from 'primeng/avatar';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { BlockUIModule } from 'primeng/blockui';
import { FormsModule } from '@angular/forms';
import { SidebarComponent } from './components/sidebar/sidebar.component';
import { LoginComponent } from './components/login/login.component';
import { SignupComponent } from './components/signup/signup.component';
import { HomeComponent } from './components/home/home.component';
import { ProjectsComponent } from './components/projects/projects.component';
import { ProfileComponent } from './components/profile/profile.component';
import { PasswordModule } from 'primeng/password';
import { DividerModule } from 'primeng/divider';
import { TooltipModule } from 'primeng/tooltip';
import { ConfirmPopupModule } from 'primeng/confirmpopup';
import { DialogModule } from 'primeng/dialog';
import { FileUploadModule } from 'primeng/fileupload';
import { ImageModule } from 'primeng/image';
import { TabViewModule } from 'primeng/tabview';
import { DropdownModule } from 'primeng/dropdown';
import { ColorPickerModule } from 'primeng/colorpicker';
import { TreeModule } from 'primeng/tree';
import { DragDropModule } from 'primeng/dragdrop';
import { ProjectItemComponent } from './components/project-item/project-item.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './auth.interceptor';

@NgModule({
  declarations: [
    AppComponent,
    SidebarComponent,
    LoginComponent,
    SignupComponent,
    HomeComponent,
    ProjectsComponent,
    ProfileComponent,
    ProjectItemComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    BrowserAnimationsModule,
    ButtonModule,
    CheckboxModule,
    InputTextModule,
    PasswordModule,
    DividerModule,
    BlockUIModule,
    AvatarModule,
    SpeedDialModule,
    ToastModule,
    TooltipModule,
    ConfirmPopupModule,
    DialogModule,
    FileUploadModule,
    ImageModule,
    TabViewModule,
    DropdownModule,
    ColorPickerModule,
    TreeModule,
    DragDropModule,
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
