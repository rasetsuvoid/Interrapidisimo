import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-skeleton',
  templateUrl: './skeleton.component.html',
  styleUrl: './skeleton.component.scss'
})
export class SkeletonComponent {
  @Input() lines = 3;
  protected get rows() { return new Array(this.lines); }
  protected readonly widths = ['100%', '85%', '70%', '92%', '60%'];
}
