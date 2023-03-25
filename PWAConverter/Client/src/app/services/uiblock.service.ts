import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UIBlockService {
  block$ = new BehaviorSubject<boolean>(false);

  blockUI(){
    this.block$.next(true);
  }

  unblockUI(){
    this.block$.next(false);
  }

}
