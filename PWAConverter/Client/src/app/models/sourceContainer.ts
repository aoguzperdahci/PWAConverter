import { TreeNode } from "primeng/api";
import { CacheStrategy } from "./cacheStrategy";
import { SourceData } from "./sourceData";

export interface SourceContainer {
  name: string;
  containerId: number;
  cacheStrategy: CacheStrategy;
  sourceList: string[];
  sourceTree: TreeNode<SourceData>[];
}
