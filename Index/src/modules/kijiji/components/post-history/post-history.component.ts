import { NestedTreeControl } from '@angular/cdk/tree';
import { HttpClient } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatTreeNestedDataSource } from '@angular/material/tree';
import { interval, Subscription } from 'rxjs';
import { UtilService } from 'src/core-services/util.service';
import { environment } from 'src/environments/environment';
import { Client, ILogTreeNode, LogTreeStatus } from '../../client.interface';

/**
 * @title Tree with nested nodes
 */
@Component({
  selector: 'app-post-history',
  templateUrl: './post-history.component.html',
  styleUrls: ['./post-history.component.css'],
})
export class PostHistoryComponent implements OnInit, OnDestroy {
  treeControl = new NestedTreeControl<ILogTreeNode>(node => node.children);
  dataSource = new MatTreeNestedDataSource<ILogTreeNode>();
  subscription?: Subscription;

  constructor(private client: Client, public utilService: UtilService,) {
  }
  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
    this.utilService.formatLocalDisplayDate
  }
  ngOnInit(): void {
    this.loadLogTreedata();
    const source = interval(60000);
    this.subscription = source.subscribe(val => {
      this.loadLogTreedata();
    });
  }

  loadLogTreedata() {
    this.client.getAll().subscribe((logs: ILogTreeNode[]) => {
      this.dataSource.data = logs;
    })
  }

  getStatusButtonClass(node: ILogTreeNode) {
    switch (node.status) {
      case LogTreeStatus._0: //pass
        return 'check_circle';
      case LogTreeStatus._1: //failed
        return 'report_problem';
      case LogTreeStatus._2: //pending
        return 'autorenew';
      case LogTreeStatus._3: //new
        return 'assignment';
      default:
        return 'assignment'; //new
    }
  }

  // hasChild = (_: number, node: ILogTreeNode) => !!node.children && node.children.length > 0;
  hasChild = (_: number, node: ILogTreeNode) => true;
}
