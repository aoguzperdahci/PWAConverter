import { Component } from '@angular/core';

@Component({
  selector: 'app-manifest-dialog',
  templateUrl: './manifest-dialog.component.html',
  styleUrls: ['./manifest-dialog.component.css']
})
export class ManifestDialogComponent {
  dialogVisible = false;
  value = "";
  name = "";
  options = ["1111111", "2222222", "33333333"]

}
